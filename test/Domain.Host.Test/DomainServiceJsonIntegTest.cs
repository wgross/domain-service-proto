using Domain.Client.Authorization;
using Domain.Client.Json;
using Domain.Contract;
using Domain.Contract.Test;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Domain.Host.Test
{
    [Collection(nameof(Service.DomainService))]
    public class DomainServiceJsonIntegTest : DomainServiceIntegTestBase
    {
        private JsonDomainClient domainClient;
        protected ServiceProvider ClientServicesProvider { get; }

        protected ServiceCollection ClientServices { get; }

        public DomainServiceJsonIntegTest()
        {
            this.ClientServices = new ServiceCollection();
            this.ClientServices.Configure<JsonDomainClientOptions>(o => o.DomainService = this.Host.Server.BaseAddress);
            this.ClientServicesProvider = this.ClientServices.BuildServiceProvider();
            this.ArrangeNewClient();
        }

        private void ArrangeNewClient()
        {
            this.domainClient = ActivatorUtilities.CreateInstance<JsonDomainClient>(this.ClientServicesProvider, this.Host.CreateDefaultClient(new DomainClientAuthorizingHandler(this.TokenProvider)));
        }

        #region Domain Command Path

        [Fact]
        public async Task JsonDomainClient_does_something()
        {
            // ARRANGE

            await this.ArrangeToken();

            // ACT

            await this.domainClient.DomainContract_does_something();
        }

        [Fact]
        public async Task JsonDomainClient_doing_something_fails_on_missing_token()
        {
            // ACT

            var result = await Assert.ThrowsAsync<InvalidOperationException>(() => this.domainClient.DomainContract_does_something());

            // ASSERT

            this.AssertUnauthorized(result);
        }

        [Fact]
        public async Task JsonDomainClient_doing_someting_fails_on_missing_body()
        {
            // ARRANGE

            await this.ArrangeToken();

            // ACT

            await this.domainClient.DomainContract_doing_someting_fails_on_missing_body();
        }

        [Fact]
        public async Task JsonDomainClient_doing_something_fails_on_bad_input()
        {
            // ARRANGE

            await this.ArrangeToken();

            // ACT

            await this.domainClient.DomainContract_doing_something_fails_on_bad_input();
        }

        [Fact]
        public async Task JsonDomainClient_creates_entity()
        {
            // ARRANGE

            await this.ArrangeToken();

            // ACT

            await this.domainClient.DomainContract_creates_entity();
        }

        [Fact]
        public async Task JsonDomainClient_creating_entity_fails_on_null_request()
        {
            // ARRANGE

            await this.ArrangeToken();

            // ACT

            await this.domainClient.DomainContract_creating_entity_fails_on_null_request();
        }

        [Fact]
        public async Task JsonDomainClient_updating_entity_fails_on_unknown_id()
        {
            // ARRANGE

            await this.ArrangeToken();
            await this.ArrangeDomainEntity();

            // ACT

            await this.domainClient.DomainContract_updating_entity_fails_on_unknown_id();
        }

        [Fact]
        public async Task JsonDomainClient_updating_entity_fails_on_missing_body()
        {
            // ARRANGE

            await this.ArrangeToken();

            // ACT

            await this.domainClient.DomainContract_updating_entity_fails_on_missing_body();
        }

        [Fact]
        public async Task JsonDomainClient_creating_entity_fails_without_token()
        {
            // ACT

            var result = await Assert.ThrowsAsync<InvalidOperationException>(() => this.domainClient.CreateEntity(new CreateDomainEntityRequest
            {
                Text = "test"
            }));

            // ASSERT

            this.AssertUnauthorized(result);
        }

        [Fact]
        public async Task JsonDomainClient_updates_entity()
        {
            // ARRANGE

            await this.ArrangeToken();
            var entity = await this.ArrangeDomainEntity();

            // ACT

            await this.domainClient.DomainContract_updates_entity(entity.Id);
        }

        [Fact]
        public async Task JsonDomainClient_updating_entity_fails_on_missing_entity()
        {
            // ARRANGE

            await this.ArrangeToken();
            var entity = await this.ArrangeDomainEntity();

            // ACT

            await this.domainClient.DomainContract_updates_entity(entity.Id);
        }

        [Fact]
        public async Task JsonDomainClient_updating_entity_fails_missing_token()
        {
            // ARRANGE

            await this.ArrangeToken();
            var entity = await this.ArrangeDomainEntity();

            this.ArrangeNewTokenProvider();
            this.ArrangeNewClient();

            // ACT

            var result = await Assert.ThrowsAsync<InvalidOperationException>(() => this.domainClient.DomainContract_updates_entity(entity.Id));

            // ASSERT

            this.AssertUnauthorized(result);
        }

        [Fact]
        public async Task JsonDomainClient_deletes_entity_by_id()
        {
            // ARRANGE

            await this.ArrangeToken();
            var entity = await this.ArrangeDomainEntity();

            // ACT

            await this.domainClient.DomainContract_deletes_entity_by_id(entity.Id);
        }

        [Fact]
        public async Task JsonDomainClient_deleting_entity_by_id_returns_false_on_missing_entity()
        {
            // ARRANGE

            await this.ArrangeToken();

            // ACT

            await this.domainClient.DomainContract_deleting_entity_returns_false_on_missing_entity();
        }

        #endregion Domain Command Path

        #region Domain Query Path

        [Fact]
        public async Task JsonDomainClient_reads_entity_by_id()
        {
            // ARRANGE

            await this.ArrangeToken();
            var entity = await this.ArrangeDomainEntity();

            // ACT

            await this.domainClient.DomainContract_reads_entity_by_id(entity.Id);
        }

        [Fact]
        public async Task JsonDomainClient_reading_entity_by_id_fails_on_unknown_id()
        {
            // ARRANGE

            await this.ArrangeToken();

            // ACT

            await this.domainClient.DomainContract_reading_entity_by_id_fails_on_unknown_id();
        }

        [Fact]
        public async Task JsonDomainClient_reading_entity_by_id_fails_with_invalid_token()
        {
            // ARRANGE

            await this.ArrangeToken();
            var entity = await this.ArrangeDomainEntity();
            await this.ArrangeInvalidToken();
            this.domainClient.HttpClient.DefaultRequestHeaders.Authorization = null;

            // ACT

            var result = await Assert.ThrowsAsync<InvalidOperationException>(() => this.domainClient.DomainContract_reads_entity_by_id(entity.Id));

            // ASSERT

            this.AssertUnauthorized(result);
        }

        [Fact]
        public async Task JsonDomainClient_reads_entities()
        {
            // ARRANGE

            await this.ArrangeToken();

            var entity = await this.ArrangeDomainEntity();

            // ACT

            await this.domainClient.DomainContract_reads_entities(entity.Id);
        }

        [Fact]
        public async Task JsonDomainClient_reads_entities_fails_on_missing_token()
        {
            // ARRANGE

            await this.ArrangeToken();
            var entity = await this.ArrangeDomainEntity();

            this.ArrangeNewTokenProvider();
            this.ArrangeNewClient();

            // ACT

            var result = await Assert.ThrowsAsync<InvalidOperationException>(() => this.domainClient.DomainContract_reads_entities(entity.Id));

            // ASSERT

            this.AssertUnauthorized(result);
        }

        #endregion Domain Query Path

        private Task<DomainEntityResult> ArrangeDomainEntity()
        {
            return this.domainClient.CreateEntity(new CreateDomainEntityRequest
            {
                Text = "test"
            });
        }

        #region Domain Events

        [Fact]
        public async Task JsonDomainClient_notifies_on_create()
        {
            // ARRANGE

            await this.ArrangeToken();

            // ACT

            await this.domainClient.DomainContract_notifies_on_create();
        }

        [Fact]
        public async Task JsonDomainClient_notifying_fails_in_missing_token()
        {
            // ACT

            var result = await Assert.ThrowsAsync<InvalidOperationException>(() => this.domainClient.DomainContract_notifies_on_create());

            // ASSERT

            this.AssertUnauthorized(result);
        }

        [Fact]
        public async Task JsonDomainClient_notifies_on_delete()
        {
            // ARRANGE

            await this.ArrangeToken();

            var createdEntity = await this.domainClient.CreateEntity(new CreateDomainEntityRequest
            {
                Text = "test-1"
            });

            await Task.Delay(500);

            // ACT

            await this.domainClient.DomainContract_notifies_on_delete(createdEntity.Id);
        }

        [Fact]
        public async Task JsonDomainClient_notifies_on_update()
        {
            // ARRANGE

            await this.ArrangeToken();

            var entity = await this.ArrangeDomainEntity();

            var createdEntity = await this.domainClient.UpdateEntity(entity.Id, new UpdateDomainEntityRequest
            {
                Text = "test-1"
            });

            await Task.Delay(500);

            // ACT

            await this.domainClient.DomainContract_notifies_on_update(createdEntity.Id);
        }

        #endregion Domain Events
    }
}