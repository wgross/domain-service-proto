using Domain.Model;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Domain.Contract.Test
{
    public static class DomainServiceContractTestBase
    {
        public static async Task DomainContract_does_something(this IDomainService domainContract)
        {
            // ACT

            var result = await domainContract.DoSomething(new Domain.Contract.DoSomethingRequest
            {
                Data = "data"
            });

            // ASSERT

            Assert.IsType<Domain.Contract.DoSomethingResult>(result);
        }

        public static async Task DomainContract_doing_someting_fails_on_missing_body(this IDomainService domainContract)
        {
            // ACT

            var result = await Assert.ThrowsAsync<ArgumentNullException>(() => domainContract.DoSomething(null));

            // ASSERT

            Assert.Equal("rq", result.ParamName);
        }

        public static async Task DomainContract_doing_something_fails_on_bad_input(this IDomainService domainContract)
        {
            // ACT

            var result = await Assert.ThrowsAsync<InvalidOperationException>(() => domainContract.DoSomething(new DoSomethingRequest
            {
                Data = null
            }));

            // ASSERT

            Assert.Equal("Data is required", result.Message);
        }

        public static async Task DomainContract_creates_entity(this IDomainService domainContract)
        {
            // ACT

            var result = await domainContract.CreateEntity(new CreateDomainEntityRequest
            {
                Text = "test"
            });

            // ASSERT

            Assert.Equal("test", result.Text);
            Assert.NotEqual(Guid.Empty, result.Id);
        }

        public static async Task DomainContract_creating_entity_fails_on_null_request(this IDomainService domainContract)
        {
            // ACT

            var result = await Assert.ThrowsAsync<ArgumentNullException>(() => domainContract.CreateEntity(null));

            // ASSERT

            Assert.Equal("createDomainEntity", result.ParamName);
        }

        public static async Task DomainContract_updates_entity(this IDomainService domainContract, Guid entityId)
        {
            // ACT

            var result = await domainContract.UpdateEntity(entityId, new UpdateDomainEntityRequest
            {
                Text = "test-changed"
            });

            // ASSERT

            Assert.Equal("test-changed", result.Text);
            Assert.Equal(entityId, result.Id);
        }

        public static async Task DomainContract_updating_entity_fails_on_missing_body(this IDomainService domainContract)
        {
            // ACT

            var result = await Assert.ThrowsAsync<ArgumentNullException>(() => domainContract.UpdateEntity(Guid.NewGuid(), null));

            // ASSERT

            Assert.Equal("updateDomainEntity", result.ParamName);
        }

        public static async Task DomainContract_updating_entity_fails_on_unknown_id(this IDomainService domainContract)
        {
            var entityId = Guid.NewGuid();

            // ACT

            var result = await Assert.ThrowsAsync<DomainEntityMissingException>(() => domainContract.UpdateEntity(entityId, new UpdateDomainEntityRequest
            {
                Text = "text-changed"
            }));

            // ASSERT

            Assert.Equal($"{nameof(DomainEntity)}(id={entityId}) not found", result.Message);
        }

        public static async Task DomainContract_reads_entity_by_id(this IDomainService domainContract, Guid id)
        {
            // ACT

            var result = await domainContract.GetEntity(id);

            // ASSERT

            Assert.Equal("test", result.Text);
            Assert.Equal(id, result.Id);
        }

        public static async Task DomainContract_deletes_entity_by_id(this IDomainService domainContract, Guid entityId)
        {
            // ACT

            await domainContract.DeleteEntity(entityId);
        }

        public static async Task DomainContract_reads_entities(this IDomainService domainContract, Guid id)
        {
            // ACT

            var result = await domainContract.GetEntities();

            // ASSERT

            Assert.Single(result.Entities);
            Assert.Equal(id, result.Entities.Single().Id);
            Assert.Equal("test", result.Entities.Single().Text);
        }

        public static async Task DomainContract_reading_entity_by_id_fails_on_unknown_id(this IDomainService domainContract)
        {
            // ACT

            var result = await domainContract.GetEntity(Guid.NewGuid());

            // ASSERT

            Assert.Null(result);
        }

        public static async Task DomainContract_notifies_on_create(this IDomainService domainContract)
        {
            // ARRANGE

            var createdEntities = new DomainEntityResult[2];
            var observer = new DomainEventCollector();

            // ACT

            var subscription = await domainContract.Subscribe(observer);

            createdEntities[0] = await domainContract.CreateEntity(new CreateDomainEntityRequest
            {
                Text = "test-1"
            });

            createdEntities[1] = await domainContract.CreateEntity(new CreateDomainEntityRequest
            {
                Text = "test-2"
            });

            await Task.Delay(1000);

            subscription.Dispose();

            // ASSERT
            // order ids to make test more robust
            Assert.Equal(
                expected: createdEntities.Select(e => e.Id).OrderBy(_ => _),
                actual: observer.Collected.Select(c => c.Id).OrderBy(_ => _));
            Assert.All(observer.Collected, c => Assert.Equal(DomainEntityEventTypes.Added, c.EventType));
        }

        public static async Task DomainContract_deleting_entity_returns_false_on_missing_entity(this IDomainService domainContract)
        {
            // ACT

            var result = await domainContract.DeleteEntity(Guid.NewGuid());

            // ASSERT

            Assert.False(result);
        }

        public static async Task DomainContract_notifies_on_delete(this IDomainService domainContract, Guid entityId)
        {
            // ARRANGE

            var observer = new DomainEventCollector();

            // ACT

            var subscription = await domainContract.Subscribe(observer);

            await domainContract.DeleteEntity(entityId);

            await Task.Delay(1000);

            subscription.Dispose();

            // ASSERT

            Assert.Equal(DomainEntityEventTypes.Deleted, observer.Collected[0].EventType);
            Assert.Equal(entityId, observer.Collected[0].Id);
        }

        public static async Task DomainContract_notifies_on_update(this IDomainService domainContract, Guid entityId)
        {
            // ARRANGE

            var observer = new DomainEventCollector();

            // ACT

            var subscription = await domainContract.Subscribe(observer);

            await domainContract.UpdateEntity(entityId, new UpdateDomainEntityRequest
            {
                Text = "test-changed"
            });

            await Task.Delay(1000);

            subscription.Dispose();

            // ASSERT

            Assert.Equal(DomainEntityEventTypes.Modified, observer.Collected[0].EventType);
            Assert.Equal(entityId, observer.Collected[0].Id);
        }
    }
}