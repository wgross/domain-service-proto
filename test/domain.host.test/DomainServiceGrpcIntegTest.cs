using domain.client.gprc;
using domain.contract;
using domain.contract.test;
using Grpc.Net.Client;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace domain.host.test
{
    public class DomainServiceGrpcIntegTest : DomainServiceContractTestBase, IDisposable
    {
        private class ResponseVersionHandler : DelegatingHandler
        {
            //https://dzone.com/articles/integration-tests-for-grpc-services-in-aspnet-core
            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var response = await base.SendAsync(request, cancellationToken);
                response.Version = request.Version;
                return response;
            }
        }

        private readonly DomainServiceTestHost host;
        private readonly GrpcChannel channel;
        private readonly GrpcDomainClient domainClient;
        private bool disposedValue;

        public DomainServiceGrpcIntegTest()
        {
            this.host = new DomainServiceTestHost();

            this.channel = GrpcChannel.ForAddress(this.host.Server.BaseAddress, new GrpcChannelOptions
            {
                HttpClient = this.host.CreateDefaultClient(new ResponseVersionHandler())
            });

            this.DomainContract = this.domainClient = new GrpcDomainClient(this.channel);
        }

        #region Dispose

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.channel.Dispose();
                    this.host.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~DomainServiceGrpcIntegTest()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion Dispose

        [Fact]
        public Task DomainServiceGrpcClient_creates_entity() => base.DomainContract_creates_entity();

        [Fact]
        public Task DomainServiceGrpcClient_creating_entity_failed_on_null_request()
            => base.DomainContract_creating_entity_fails_on_null_request();

        private Task<DomainEntityResult> ArrangeDomainEntity()
        {
            return this.DomainContract.CreateEntity(new CreateDomainEntityRequest
            {
                Text = "test"
            });
        }

        [Fact]
        public async Task DomainServiceGrpcClient_deletes_entity_by_id()
        {
            // ARRANGE

            var entity = await ArrangeDomainEntity();

            // ACT

            await base.DomainContract_deletes_entity_by_id(entity.Id);
        }

        [Fact]
        public Task DomainServiceGrpcClient_deleting_entity_by_id_returns_false_on_missing_entity()
          => base.DomainContract_deleting_entity_by_id_returns_false_on_missing_entity();
    }
}