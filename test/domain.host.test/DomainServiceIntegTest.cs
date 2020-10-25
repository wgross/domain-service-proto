using domain.client;
using domain.contract;
using domain.contract.test;
using System.Threading.Tasks;
using Xunit;

namespace domain.host.test
{
    public class DomainServiceIntegTest : DomainServiceContractTestBase
    {
        private readonly DomainServiceTestHost host;
        private readonly DomainClient domainClient;

        public DomainServiceIntegTest()
        {
            this.host = new DomainServiceTestHost();
            this.domainClient = new DomainClient(this.host.CreateClient());
            this.Contract = this.domainClient;
        }

        #region Test Domain Services Command Path

        [Fact]
        public Task DomainService_does_something() => base.ACT_DomainService_does_something();

        [Fact]
        public Task DomainService_doing_someting_fails_on_missing_body() => base.ACT_DomainService_doing_someting_fails_on_missing_body();

        [Fact]
        public Task DomainService_doing_something_fails_on_bad_input() => base.ACT_DomainService_doing_something_fails_on_bad_input();

        [Fact]
        public Task DomainService_creates_entity() => base.ACT_DomainService_creates_entity();

        [Fact]
        public async Task DomainService_reads_entity_by_id()
        {
            // ARRANGE

            var entity = await this.ArrangeDomainEntity();

            // ACT

            await base.ACT_DomainService_reads_entity_by_id(entity.Id);
        }

        [Fact]
        public async Task DomainService_reading_entity_by_id_fails_on_unknown_id()
        {
            // ACT

            await base.ACT_DomainService_reading_entity_by_id_fails_on_unknown_id();
        }

        [Fact]
        public async Task DomainService_reads_entities()
        {
            // ARRANGE

            var entity = await this.ArrangeDomainEntity();

            // ACT

            await base.ACT_DomainService_reads_entities(entity.Id);
        }

        [Fact]
        public async Task DomainService_deletes_entity_by_id()
        {
            // ARRANGE

            var entity = await ArrangeDomainEntity();

            // ACT

            await base.ACT_DomainService_deletes_entity_by_id(entity.Id);
        }

        #endregion Test Domain Services Command Path

        private Task<DomainEntityResult> ArrangeDomainEntity()
        {
            return this.Contract.CreateEntity(new CreateDomainEntityRequest
            {
                Text = "test"
            });
        }

        #region Test Domain Services Events

        [Fact]
        public Task DomainService_notifies_on_create() => base.ACT_DomainService_notifies_on_create();

        //{
        //    // ARRANGE

        //    var createdEntities = new DomainEntityResult[2];
        //    var events = new DomainEventCollector();
        //    var stopListening = new CancellationTokenSource();

        //    // ACT

        //    var eventTask = this.Contract.SubscribeAndListen(stopListening.Token, events);

        //    createdEntities[0] = await this.Contract.CreateEntity(new CreateDomainEntityRequest
        //    {
        //        Text = "test-1"
        //    });

        //    createdEntities[1] = await this.Contract.CreateEntity(new CreateDomainEntityRequest
        //    {
        //        Text = "test-2"
        //    });

        //    await Task.Delay(1000); // give time to deliver
        //    stopListening.Cancel();
        //    await eventTask;

        //    // ASSERT

        //    Assert.Equal(DomainEntityEventTypes.Added, events.Collected[0].EventType);
        //    Assert.Equal(createdEntities[0].Id, events.Collected[0].Id);

        //    Assert.Equal(DomainEntityEventTypes.Added, events.Collected[1].EventType);
        //    Assert.Equal(createdEntities[1].Id, events.Collected[1].Id);
        //}

        [Fact]
        public async Task DomainService_notifies_on_delete()
        {
            // ARRANGE

            var createdEntity = await this.Contract.CreateEntity(new CreateDomainEntityRequest
            {
                Text = "test-1"
            });

            // ACT

            await base.ACT_DomainService_notifies_on_delete(createdEntity.Id);
        }

        //{
        //    // ARRANGE

        //    var createdEntity = await this.Contract.CreateEntity(new CreateDomainEntityRequest
        //    {
        //        Text = "test-1"
        //    });

        //    var events = new DomainEventCollector();
        //    var stopListening = new CancellationTokenSource();

        //    // ACT

        //    var eventTask = this.domainClient.SubscribeAndListen(stopListening.Token, events);

        //    await this.domainClient.DeleteEntity(createdEntity.Id);

        //    await Task.Delay(1000); // give time to deliver
        //    stopListening.Cancel();
        //    await eventTask;

        //    // ASSERT

        //    Assert.Equal(DomainEntityEventTypes.Deleted, events.Collected[0].EventType);
        //    Assert.Equal(createdEntity.Id, events.Collected[0].Id);
        //}

        #endregion Test Domain Services Events
    }
}