using Domain.Client.Gprc;
using Domain.Contract;
using Domain.Contract.Test;
using Grpc.Net.Client;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Domain.Host.Test
{
    [Collection(nameof(Service.DomainService))]
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

        #region Domain Command Path

        [Fact]
        public Task GrpcDomainClient_creates_entity() => base.DomainContract_creates_entity();

        [Fact]
        public Task GrpcDomainClient_creating_entity_failed_on_null_request()
            => base.DomainContract_creating_entity_fails_on_null_request();

        private Task<DomainEntityResult> ArrangeDomainEntity()
        {
            return this.DomainContract.CreateEntity(new CreateDomainEntityRequest
            {
                Text = "test"
            });
        }

        [Fact]
        public async Task GrpcDomainClient_updates_entity()
        {
            // ARRANGE

            var entity = await this.ArrangeDomainEntity();

            // ACT

            await this.DomainContract_updates_entity(entity.Id);
        }

        [Fact]
        public async Task GrpcDomainClient_updating_entity_fails_on_unknown_id()
        {
            // ACT

            await this.DomainContract_updating_entity_fails_on_unknown_id();
        }

        [Fact]
        public async Task GrpcDomainClient_updating_entity_fails_on_missing_body()
        {
            await this.DomainContract_updating_entity_fails_on_missing_body();
        }

        [Fact]
        public async Task GrpcDomainClient_deletes_entity_by_id()
        {
            // ARRANGE

            var entity = await ArrangeDomainEntity();

            // ACT

            await base.DomainContract_deletes_entity_by_id(entity.Id);
        }

        [Fact]
        public Task GrpcDomainClient_deleting_entity_by_id_returns_false_on_missing_entity()
          => base.DomainContract_deleting_entity_returns_false_on_missing_entity();

        #endregion Domain Command Path

        #region Domain Query Path

        [Fact]
        public async Task GrpcDomainClient_reads_entity_by_id()
        {
            // ARRANGE

            var entity = await this.ArrangeDomainEntity();

            // ACT

            await base.DomainContract_reads_entity_by_id(entity.Id);
        }

        [Fact]
        public async Task GrpcDomainClient_reading_entity_by_id_fails_on_unknown_id()
        {
            // ACT

            await base.DomainContract_reading_entity_by_id_fails_on_unknown_id();
        }

        [Fact]
        public async Task GrpcDomainClient_reads_entities()
        {
            // ARRANGE

            var entity = await this.ArrangeDomainEntity();

            // ACT

            await base.DomainContract_reads_entities(entity.Id);
        }

        #endregion Domain Query Path

        #region Domain Events

        [Fact]
        public async Task GrpcDomainClient_notifies_on_create()
        {
            // ACT

            await base.DomainContract_notifies_on_create();
        }

        [Fact]
        public async Task GrpcDomainClient_notifies_on_delete()
        {
            // ARRANGE

            var entity = await this.ArrangeDomainEntity();

            await Task.Delay(500);

            // ACT

            await base.DomainContract_notifies_on_delete(entity.Id);
        }

        [Fact]
        public async Task GrpcDomainClient_notifies_on_update()
        {
            // ARRANGE

            var entity = await this.ArrangeDomainEntity();

            await Task.Delay(500);

            // ACT

            await base.DomainContract_notifies_on_update(entity.Id);
        }

        #endregion Domain Events
    }
}