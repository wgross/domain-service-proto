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
    public class DomainServiceGrpcIntegTest : DomainServiceIntegTestBase, IDisposable
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

        private GrpcChannel channel;
        private GrpcDomainClient domainClient;
        private bool disposedValue;

        public DomainServiceGrpcIntegTest()
        {
            this.ArrangeNewClient();
        }

        #region Arrange Test Environment

        private void ArrangeNewClient()
        {
            this.channel = GrpcChannel.ForAddress(this.Host.Server.BaseAddress, new GrpcChannelOptions
            {
                HttpClient = this.Host.CreateDefaultClient(new ResponseVersionHandler())
            });
            this.domainClient = new GrpcDomainClient(this.channel, this.TokenProvider);
        }

        private Task<DomainEntityResult> ArrangeDomainEntity()
        {
            return this.domainClient.CreateEntity(new CreateDomainEntityRequest
            {
                Text = "test"
            });
        }

        #endregion Arrange Test Environment

        #region Dispose

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.channel.Dispose();
                    this.Host.Dispose();
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
        public async Task GrpcDomainClient_creates_entity()
        {
            // ARRANGE

            await this.ArrangeToken();

            // ACT

            await this.domainClient.DomainContract_creates_entity();
        }

        [Fact]
        public async Task GrpcDomainClient_creating_entity_failed_on_null_request()
        {
            // ARRANGE

            await this.ArrangeToken();

            // ACT

            await this.domainClient.DomainContract_creating_entity_fails_on_null_request();
        }

        [Fact]
        public async Task GrpcDomainClient_creating_entity_fails_on_missing_token()
        {
            // ACT

            var result = await Assert.ThrowsAsync<InvalidOperationException>(() => this.domainClient.DomainContract_creates_entity());

            // ASSERT

            this.AssertUnauthorized(result);
        }

        [Fact]
        public async Task GrpcDomainClient_updates_entity()
        {
            // ARRANGE

            await this.ArrangeToken();

            var entity = await this.ArrangeDomainEntity();

            // ACT

            await this.domainClient.DomainContract_updates_entity(entity.Id);
        }

        [Fact]
        public async Task GrpcDomainClient_updating_entity_fails_on_unknown_id()
        {
            // ARRANGE

            await this.ArrangeToken();

            // ACT

            await this.domainClient.DomainContract_updating_entity_fails_on_unknown_id();
        }

        [Fact]
        public async Task GrpcDomainClient_updating_entity_fails_on_missing_body()
        {
            // ARRANGE

            await this.ArrangeToken();

            // ACT

            await this.domainClient.DomainContract_updating_entity_fails_on_missing_body();
        }

        [Fact]
        public async Task GrpcDomainClient_updating_entity_fails_on_missing_token()
        {
            // ARRANGE

            await this.ArrangeToken();

            var entity = await this.ArrangeDomainEntity();

            this.ArrangeNewTokenProvider();
            this.ArrangeNewClient();

            // ACT

            var result = Assert.ThrowsAsync<InvalidOperationException>(() => this.domainClient.DomainContract_updates_entity(entity.Id));
        }

        [Fact]
        public async Task GrpcDomainClient_deletes_entity_by_id()
        {
            // ARRANGE

            await this.ArrangeToken();

            var entity = await ArrangeDomainEntity();

            // ACT

            await this.domainClient.DomainContract_deletes_entity_by_id(entity.Id);
        }

        [Fact]
        public async Task GrpcDomainClient_deleting_entity_by_id_returns_false_on_missing_entity()
        {
            // ARRANGE

            await this.ArrangeToken();

            // ARRANGE

            await this.domainClient.DomainContract_deleting_entity_returns_false_on_missing_entity();
        }

        [Fact]
        public async Task GrpcDomainClient_deleting_entity_by_id_fails_on_missing_token()
        {
            // ARRANGE

            await this.ArrangeToken();

            var entity = await ArrangeDomainEntity();

            this.ArrangeNewTokenProvider();
            this.ArrangeNewClient();

            // ACT

            var result = await Assert.ThrowsAsync<InvalidOperationException>(() => this.domainClient.DomainContract_deletes_entity_by_id(entity.Id));

            // ASSERT

            this.AssertUnauthorized(result);
        }

        #endregion Domain Command Path

        #region Domain Query Path

        [Fact]
        public async Task GrpcDomainClient_reads_entity_by_id()
        {
            // ARRANGE

            await this.ArrangeToken();

            var entity = await this.ArrangeDomainEntity();

            this.ArrangeNewTokenProvider();
            this.ArrangeNewClient();

            // ACT

            var result = await Assert.ThrowsAsync<InvalidOperationException>(() => this.domainClient.DomainContract_reads_entity_by_id(entity.Id));

            // ASSERT

            this.AssertUnauthorized(result);
        }

        [Fact]
        public async Task GrpcDomainClient_reading_entity_by_id_fails_on_unknown_id()
        {
            // ARRANGE

            await this.ArrangeToken();

            // ACT

            await this.domainClient.DomainContract_reading_entity_by_id_fails_on_unknown_id();
        }

        [Fact]
        public async Task GrpcDomainClient_reads_entities()
        {
            // ARRANGE

            await this.ArrangeToken();

            var entity = await this.ArrangeDomainEntity();

            // ACT

            await this.domainClient.DomainContract_reads_entities(entity.Id);
        }

        [Fact]
        public async Task GrpcDomainClient_reading_entities_fails_on_missing_token()
        {
            // ARRANGE

            await this.ArrangeToken();

            var entity = await this.ArrangeDomainEntity();

            this.ArrangeNewTokenProvider();
            this.ArrangeNewClient();

            // ACT

            var result = await Assert.ThrowsAsync<InvalidOperationException>(() => this.domainClient.DomainContract_reads_entities(entity.Id));

            this.AssertUnauthorized(result);
        }

        #endregion Domain Query Path

        #region Domain Events

        [Fact]
        public async Task GrpcDomainClient_notifies_on_create()
        {
            // ARRANGE

            await this.ArrangeToken();

            // ACT

            await this.domainClient.DomainContract_notifies_on_create();
        }

        [Fact]
        public async Task GrpcDomainClient_notifying_fails_on_missing_token()
        {
            // ACT

            var result = await Assert.ThrowsAsync<InvalidOperationException>(() => this.domainClient.DomainContract_notifies_on_create());

            this.AssertUnauthorized(result);
        }

        [Fact]
        public async Task GrpcDomainClient_notifies_on_delete()
        {
            // ARRANGE

            await this.ArrangeToken();

            // ARRANGE

            var entity = await this.ArrangeDomainEntity();

            await Task.Delay(500);

            // ACT

            await this.domainClient.DomainContract_notifies_on_delete(entity.Id);
        }

        [Fact]
        public async Task GrpcDomainClient_notifies_on_update()
        {
            // ARRANGE

            await this.ArrangeToken();

            // ARRANGE

            var entity = await this.ArrangeDomainEntity();

            await Task.Delay(500);

            // ACT

            await this.domainClient.DomainContract_notifies_on_update(entity.Id);
        }

        #endregion Domain Events
    }
}