using Domain.Client;
using Domain.Contract;
using Domain.Contract.Test;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;

namespace Domain.Host.Test
{
    [Collection(nameof(Service.DomainService))]
    public class DomainServiceHttpIntegTest : DomainServiceContractTestBase
    {
        private readonly DomainServiceTestHost host;
        private readonly JsonDomainClient domainClient;
        private readonly ServiceProvider clientServiceProvider;
        private ServiceCollection clientServices;

        public DomainServiceHttpIntegTest()
        {
            this.host = new DomainServiceTestHost();
            this.DomainContract = this.domainClient = new JsonDomainClient(this.host.CreateClient(), new JsonDomainClientOptions { DomainService = this.host.Server.BaseAddress });
        }

        #region Domain Command Path

        [Fact]
        public Task JsonDomainClient_does_something() => base.DomainContract_does_something();

        [Fact]
        public Task JsonDomainClient_doing_someting_fails_on_missing_body() => base.DomainContract_doing_someting_fails_on_missing_body();

        [Fact]
        public Task JsonDomainClient_doing_something_fails_on_bad_input() => base.DomainContract_doing_something_fails_on_bad_input();

        [Fact]
        public Task JsonDomainClient_creates_entity() => base.DomainContract_creates_entity();

        [Fact]
        public Task JsonDomainClient_creating_entity_fails_on_null_request() => base.DomainContract_creating_entity_fails_on_null_request();

        [Fact]
        public async Task JsonDomainClient_updating_entity_fails_on_unknown_id()
        {
            // ARRANGE

            var entity = await this.ArrangeDomainEntity();

            // ACT

            await this.DomainContract_updating_entity_fails_on_unknown_id();
        }

        [Fact]
        public async Task JsonDomainClient_updating_entity_fails_on_missing_body()
        {
            await this.DomainContract_updating_entity_fails_on_missing_body();
        }

        [Fact]
        public async Task JsonDomainClient_updates_entity()
        {
            // ARRANGE

            var entity = await this.ArrangeDomainEntity();

            // ACT

            await this.DomainContract_updates_entity(entity.Id);
        }

        [Fact]
        public async Task JsonDomainClient_updating_entity_fails_on_missing_entity()
        {
            // ARRANGE

            var entity = await this.ArrangeDomainEntity();

            // ACT

            await this.DomainContract_updates_entity(entity.Id);
        }

        [Fact]
        public async Task JsonDomainClient_deletes_entity_by_id()
        {
            // ARRANGE

            var entity = await this.ArrangeDomainEntity();

            // ACT

            await base.DomainContract_deletes_entity_by_id(entity.Id);
        }

        [Fact]
        public Task JsonDomainClient_deleting_entity_by_id_returns_false_on_missing_entity()
            => base.DomainContract_deleting_entity_returns_false_on_missing_entity();

        #endregion Domain Command Path

        #region Domain Query Path

        [Fact]
        public async Task JsonDomainClient_reads_entity_by_id()
        {
            // ARRANGE

            var entity = await this.ArrangeDomainEntity();

            // ACT

            await base.DomainContract_reads_entity_by_id(entity.Id);
        }

        [Fact]
        public async Task JsonDomainClient_reading_entity_by_id_fails_on_unknown_id()
        {
            // ACT

            await base.DomainContract_reading_entity_by_id_fails_on_unknown_id();
        }

        [Fact]
        public async Task JsonDomainClient_reads_entities()
        {
            // ARRANGE

            var entity = await this.ArrangeDomainEntity();

            // ACT

            await base.DomainContract_reads_entities(entity.Id);
        }

        #endregion Domain Query Path

        private Task<DomainEntityResult> ArrangeDomainEntity()
        {
            return this.DomainContract.CreateEntity(new CreateDomainEntityRequest
            {
                Text = "test"
            });
        }

        #region Domain Events

        [Fact]
        public Task JsonDomainClient_notifies_on_create() => base.DomainContract_notifies_on_create();

        [Fact]
        public async Task JsonDomainClient_notifies_on_delete()
        {
            // ARRANGE

            var createdEntity = await this.DomainContract.CreateEntity(new CreateDomainEntityRequest
            {
                Text = "test-1"
            });

            await Task.Delay(500);

            // ACT

            await base.DomainContract_notifies_on_delete(createdEntity.Id);
        }

        [Fact]
        public async Task JsonDomainClient_notifies_on_update()
        {
            // ARRANGE

            var entity = await this.ArrangeDomainEntity();

            var createdEntity = await this.DomainContract.UpdateEntity(entity.Id, new UpdateDomainEntityRequest
            {
                Text = "test-1"
            });

            await Task.Delay(500);

            // ACT

            await base.DomainContract_notifies_on_update(createdEntity.Id);
        }

        #endregion Domain Events
    }
}