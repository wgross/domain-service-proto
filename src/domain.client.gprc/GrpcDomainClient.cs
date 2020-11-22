using Domain.Contract;
using Grpc.Net.Client;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using static Domain.Contract.Proto.GDomainService;

namespace Domain.Client.Gprc
{
    public class GrpcDomainClient : GDomainServiceClient, IDomainService
    {
        private HttpClient httpClient;

        public GrpcDomainClient(GrpcChannel channel)
            : base(channel)
        { }

        public GrpcDomainClient(HttpClient httpClient)
            : base()
        {
            this.httpClient = httpClient;
        }

        public async Task<DomainEntityResult> CreateEntity(CreateDomainEntityRequest createDomainEntity)
        {
            if (createDomainEntity is null)
                throw new ArgumentNullException(nameof(createDomainEntity));

            return (await base.CreateAsync(createDomainEntity.ToGrpcMessage())).FromGrpcMessage();
        }

        public async Task<bool> DeleteEntity(Guid entityId)
        {
            return (await base.DeleteAsync(entityId.ToGrpcMessage())).FromGrpcMessage();
        }

        public Task<DoSomethingResult> DoSomething(DoSomethingRequest rq)
        {
            throw new NotImplementedException();
        }

        public Task<DomainEntityCollectionResult> GetEntities()
        {
            throw new NotImplementedException();
        }

        public Task<DomainEntityResult> GetEntity(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IDisposable> Subscribe(IObserver<DomainEntityEvent> events)
        {
            throw new NotImplementedException();
        }
    }
}