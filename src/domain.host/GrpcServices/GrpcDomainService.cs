using Domain.Contract;
using Domain.Contract.Proto;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Channels;
using System.Threading.Tasks;
using static Domain.Contract.Proto.GDomainService;

namespace Domain.Host.GrpcServices
{
    [Authorize]
    public class GrpcDomainService : GDomainServiceBase, IObserver<DomainEntityEvent>
    {
        private readonly IDomainService domainService;

        public GrpcDomainService(IDomainService domainService)
        {
            this.domainService = domainService;
        }

        public override async Task<GrpcDomainEntityResponse> Create(GrpcCreateDomainEntityRequest request, ServerCallContext context)
        {
            return (await this.domainService.CreateEntity(request.FromGrpcMessage()).ConfigureAwait(false)).ToGrpcMessage();
        }

        public override async Task<GrpcDomainEntityCollectionResponse> GetAll(GrpcGetAllDomainEntitiesRequest request, ServerCallContext context)
        {
            return (await this.domainService.GetEntities().ConfigureAwait(false)).ToGrpcMessage();
        }

        public override async Task<GrpcDomainEntityResponse> Get(GrpcGetDomainEntityByIdRequest request, ServerCallContext context)
        {
            var result = await this.domainService.GetEntity(request.FromGrpcMessage()).ConfigureAwait(false);
            if (result is null)
                throw new RpcException(new Status(StatusCode.NotFound, "Entity doesn't exist"));

            return result.ToGrpcMessage();
        }

        public override async Task<GrpcDomainEntityResponse> Update(GrpcUpdateDomainEntityRequest request, ServerCallContext context)
        {
            try
            {
                return (await this.domainService.UpdateEntity(Guid.Parse(request.Id), request.FromGrpcMessage()).ConfigureAwait(false)).ToGrpcMessage();
            }
            catch (DomainEntityMissingException ex)
            {
                throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
            }
        }

        public override async Task<GrpcDeleteDomainEntityResponse> Delete(GrpcDeleteDomainEntityRequest request, ServerCallContext context)
        {
            return (await this.domainService.DeleteEntity(request.FromGrpcMessage()).ConfigureAwait(false)).ToGrpcMessage();
        }

        #region Domain Events

        private class DomainEventChannelWriter : IObserver<DomainEntityEvent>, IDisposable
        {
            private ChannelWriter<DomainEntityEvent> writer;

            public DomainEventChannelWriter(ChannelWriter<DomainEntityEvent> writer)
            {
                this.writer = writer;
            }

            public void Dispose()
            {
                this.writer?.Complete();
                this.writer = null;
            }

            public void OnCompleted() => this.writer.Complete();

            public void OnError(Exception error)
            {
            }

            public void OnNext(DomainEntityEvent value) => this.writer.WriteAsync(value).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public override async Task Subscribe(GrpcSubscribeDomainEvent request, IServerStreamWriter<GrpcDomainEntityEvent> responseStream, ServerCallContext context)
        {
            var channel = Channel.CreateUnbounded<DomainEntityEvent>();

            using var subscription = this.domainService.Subscribe(new DomainEventChannelWriter(channel.Writer));

            try
            {
                while (!context.CancellationToken.IsCancellationRequested)
                {
                    // wait for an event
                    if (await channel.Reader.WaitToReadAsync(context.CancellationToken).ConfigureAwait(false))
                    {
                        var nextEvent = await channel.Reader.ReadAsync().ConfigureAwait(false);
                        await responseStream.WriteAsync(nextEvent.ToGrpcMessage()).ConfigureAwait(false);
                    }
                }
            }
            catch (OperationCanceledException)
            { }
        }

        void IObserver<DomainEntityEvent>.OnCompleted()
        {
            throw new NotImplementedException();
        }

        void IObserver<DomainEntityEvent>.OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        void IObserver<DomainEntityEvent>.OnNext(DomainEntityEvent value)
        {
            throw new NotImplementedException();
        }

        #endregion Domain Events
    }
}