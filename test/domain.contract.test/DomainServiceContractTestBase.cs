using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Domain.Contract.Test
{
    public abstract class DomainServiceContractTestBase
    {
        protected IDomainService DomainContract { get; set; }

        protected async Task DomainContract_does_something()
        {
            // ACT

            var result = await this.DomainContract.DoSomething(new Domain.Contract.DoSomethingRequest
            {
                Data = "data"
            });

            // ASSERT

            Assert.IsType<Domain.Contract.DoSomethingResult>(result);
        }

        protected async Task DomainContract_doing_someting_fails_on_missing_body()
        {
            // ACT

            var result = await Assert.ThrowsAsync<ArgumentNullException>(() => this.DomainContract.DoSomething(null));

            // ASSERT

            Assert.Equal("rq", result.ParamName);
        }

        protected async Task DomainContract_doing_something_fails_on_bad_input()
        {
            // ACT

            var result = await Assert.ThrowsAsync<InvalidOperationException>(() => this.DomainContract.DoSomething(new DoSomethingRequest
            {
                Data = null
            }));

            // ASSERT

            Assert.Equal("Data is required", result.Message);
        }

        protected async Task DomainContract_creates_entity()
        {
            // ACT

            var result = await this.DomainContract.CreateEntity(new CreateDomainEntityRequest
            {
                Text = "test"
            });

            // ASSERT

            Assert.Equal("test", result.Text);
            Assert.NotEqual(Guid.Empty, result.Id);
        }

        protected async Task DomainContract_creating_entity_fails_on_null_request()
        {
            // ACT

            var result = await Assert.ThrowsAsync<ArgumentNullException>(() => this.DomainContract.CreateEntity(null));

            // ASSERT

            Assert.Equal("createDomainEntity", result.ParamName);
        }

        protected async Task DomainContract_reads_entity_by_id(Guid id)
        {
            // ACT

            var result = await this.DomainContract.GetEntity(id);

            // ASSERT

            Assert.Equal("test", result.Text);
            Assert.Equal(id, result.Id);
        }

        public async Task DomainContract_deletes_entity_by_id(Guid entityId)
        {
            // ACT

            await this.DomainContract.DeleteEntity(entityId);
        }

        public async Task DomainContract_reads_entities(Guid id)
        {
            // ACT

            var result = await this.DomainContract.GetEntities();

            // ASSERT

            Assert.Single(result.Entities);
            Assert.Equal(id, result.Entities.Single().Id);
            Assert.Equal("test", result.Entities.Single().Text);
        }

        public async Task DomainContract_reading_entity_by_id_fails_on_unknown_id()
        {
            // ACT

            var result = await this.DomainContract.GetEntity(Guid.NewGuid());

            // ASSERT

            Assert.Null(result);
        }

        public async Task DomainContract_notifies_on_create()
        {
            // ARRANGE

            var createdEntities = new DomainEntityResult[2];
            var observer = new DomainEventCollector();

            // ACT

            var subscription = await this.DomainContract.Subscribe(observer);

            createdEntities[0] = await this.DomainContract.CreateEntity(new CreateDomainEntityRequest
            {
                Text = "test-1"
            });

            createdEntities[1] = await this.DomainContract.CreateEntity(new CreateDomainEntityRequest
            {
                Text = "test-2"
            });

            await Task.Delay(1000);

            subscription.Dispose();

            // ASSERT

            Assert.Equal(DomainEntityEventTypes.Added, observer.Collected[0].EventType);
            Assert.Equal(createdEntities[0].Id, observer.Collected[0].Id);

            Assert.Equal(DomainEntityEventTypes.Added, observer.Collected[1].EventType);
            Assert.Equal(createdEntities[1].Id, observer.Collected[1].Id);
        }

        public async Task DomainContract_deleting_entity_by_id_returns_false_on_missing_entity()
        {
            // ACT

            var result = await this.DomainContract.DeleteEntity(Guid.NewGuid());

            // ASSERT

            Assert.False(result);
        }

        public async Task DomainContract_notifies_on_delete(Guid entityId)
        {
            // ARRANGE

            var observer = new DomainEventCollector();

            // ACT

            var subscription = await this.DomainContract.Subscribe(observer);

            await this.DomainContract.DeleteEntity(entityId);

            await Task.Delay(1000);

            subscription.Dispose();

            // ASSERT

            Assert.Equal(DomainEntityEventTypes.Deleted, observer.Collected[0].EventType);
            Assert.Equal(entityId, observer.Collected[0].Id);
        }
    }
}