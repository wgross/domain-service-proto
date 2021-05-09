using Domain.Client.Authorization;
using Domain.Contract;
using Domain.Contract.Proto;
using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using static Domain.Contract.Proto.GDomainService;

namespace Domain.Client.Gprc
{
    /// <summary>
    /// Implements the GRPC client of the domian service.
    /// </summary>
    public class GrpcDomainClient : GDomainServiceClient, IDomainService
    {
        private IDomainClientTokenProvider tokenProvider;

        public GrpcDomainClient(GrpcChannel channel, IDomainClientTokenProvider tokenProvider)
            : base(channel)
        {
            this.tokenProvider = tokenProvider;
        }

        internal Metadata GetMetadata()
        {
            var headers = new Metadata();
            headers.Add("Authorization", $"Bearer {this.tokenProvider.GetAccessToken()}");
            return headers;
        }

        /// <inheritdoc/>
        public async Task<DomainEntityResult> CreateEntity(CreateDomainEntityRequest createDomainEntity)
        {
            if (createDomainEntity is null)
                throw new ArgumentNullException(nameof(createDomainEntity));

            return (await base.CreateAsync(createDomainEntity.ToGrpcMessage(), this.GetMetadata())).FromGrpcMessage();
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteEntity(Guid entityId)
        {
            return (await base.DeleteAsync(
                request: new GrpcDeleteDomainEntityRequest
                {
                    Id = entityId.ToString()
                },
                headers: this.GetMetadata())).FromGrpcMessage();
        }

        /// <inheritdoc/>
        public Task<DoSomethingResult> DoSomething(DoSomethingRequest rq)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public async Task<DomainEntityCollectionResult> GetEntities()
        {
            return (await base.GetAllAsync(new GrpcGetAllDomainEntitiesRequest(), this.GetMetadata())).FromGrpcMessage();
        }

        /// <inheritdoc/>
        public async Task<DomainEntityResult> GetEntity(Guid id)
        {
            try
            {
                return (await base.GetAsync(new GrpcGetDomainEntityByIdRequest { Id = id.ToString() }, this.GetMetadata())).FromGrpcMessage();
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                return null;
            }
        }

        /// <inheritdoc/>
        public async Task<DomainEntityResult> UpdateEntity(Guid id, UpdateDomainEntityRequest updateDomainEntity)
        {
            if (updateDomainEntity is null)
                throw new ArgumentNullException(nameof(updateDomainEntity));

            try
            {
                return (await base.UpdateAsync(updateDomainEntity.ToGrpcMessage(id), this.GetMetadata())).FromGrpcMessage();
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                throw new DomainEntityMissingException(ex.Status.Detail);
            }
        }

        #region DomainEvent

        private class DomainEventReceiver : IDisposable
        {
            private readonly ISubject<DomainEntityEvent> IncomingEvents = new Subject<DomainEntityEvent>();
            private readonly CancellationTokenSource cancelListening = new CancellationTokenSource();
            private GDomainServiceClient grpcClient;

            public DomainEventReceiver(GDomainServiceClient grpcClient)
            {
                this.grpcClient = grpcClient;
            }

            internal async Task SubscribeAndListen(IObserver<DomainEntityEvent> observer, Metadata metadata)
            {
                // the background thread will run until the cancellation breaks it.
                await Task
                    .Run(async () =>
                    {
                        // subscribe the external observer to the local subject as long as the loop is running
                        using var subscription = this.IncomingEvents.Subscribe(observer);

                        try
                        {
                            // start the call and forward incoming domain events to the observer
                            using var streamingCall = this.grpcClient.Subscribe(new GrpcSubscribeDomainEvent(), metadata, cancellationToken: this.cancelListening.Token);

                            while (await streamingCall.ResponseStream.MoveNext() && !this.cancelListening.IsCancellationRequested)
                            {
                                var entityEvent = streamingCall.ResponseStream.Current;
                                this.IncomingEvents.OnNext(entityEvent.FromGrpcMessage());
                            }
                        }
                        catch (OperationCanceledException)
                        {
                            return;
                        }
                    })
                    .ConfigureAwait(false);
            }

            public void Dispose()
            {
                this.cancelListening.Cancel();
                this.grpcClient = null;
            }
        }

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        public Task<IDisposable> Subscribe(IObserver<DomainEntityEvent> observer)
        {
            var tmp = new DomainEventReceiver(this);

            tmp.SubscribeAndListen(observer, this.GetMetadata());
            return Task.FromResult((IDisposable)tmp);
        }

#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        #endregion DomainEvent
    }
}