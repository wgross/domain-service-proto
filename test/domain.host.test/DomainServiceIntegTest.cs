using domain.client;
using domain.client.gprc;
using domain.contract;
using domain.contract.test;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;

namespace domain.host.test
{
    public class DomainServiceIntegTest : DomainServiceContractTestBase
    {
        private readonly DomainServiceTestHost host;
        private readonly DomainClient domainClient;
        private readonly ServiceProvider clientServiceProvider;
        private ServiceCollection clientServices;

        public DomainServiceIntegTest()
        {
            this.host = new DomainServiceTestHost();
            this.DomainContract = this.domainClient = new DomainClient(this.host.CreateClient());
        }

        #region Domain Command Path

        [Fact]
        public Task DomainClient_does_something() => base.DomainContract_does_something();

        [Fact]
        public Task DomainClient_doing_someting_fails_on_missing_body() => base.DomainContract_doing_someting_fails_on_missing_body();

        [Fact]
        public Task DomainClient_doing_something_fails_on_bad_input() => base.DomainContract_doing_something_fails_on_bad_input();

        [Fact]
        public Task DomainClient_creates_entity() => base.DomainContract_creates_entity();

        [Fact]
        public Task DomainClient_creating_entity_fails_on_null_data() => base.DomainContract_creating_entity_fails_on_null_request();

        [Fact]
        public async Task DomainClient_deletes_entity_by_id()
        {
            // ARRANGE

            var entity = await ArrangeDomainEntity();

            // ACT

            await base.DomainContract_deletes_entity_by_id(entity.Id);
        }

        [Fact]
        public Task DomainClient_deleting_entity_by_id_returns_false_on_missing_entity()
            => base.DomainContract_deleting_entity_by_id_returns_false_on_missing_entity();

        #endregion Domain Command Path

        #region Domain Query Path

        [Fact]
        public async Task DomainClient_reads_entity_by_id()
        {
            // ARRANGE

            var entity = await this.ArrangeDomainEntity();

            // ACT

            await base.DomainContract_reads_entity_by_id(entity.Id);
        }

        [Fact]
        public async Task DomainClient_reading_entity_by_id_fails_on_unknown_id()
        {
            // ACT

            await base.DomainContract_reading_entity_by_id_fails_on_unknown_id();
        }

        [Fact]
        public async Task DomainClient_reads_entities()
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
        public Task DomainClient_notifies_on_create() => base.DomainContract_notifies_on_create();

        [Fact]
        public async Task DomainClient_notifies_on_delete()
        {
            // ARRANGE

            var createdEntity = await this.DomainContract.CreateEntity(new CreateDomainEntityRequest
            {
                Text = "test-1"
            });

            // ACT

            await base.DomainContract_notifies_on_delete(createdEntity.Id);
        }

        #endregion Domain Events
    }
}