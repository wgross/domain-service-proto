using domain.client;
using domain.contract;
using domain.contract.test;
using domain.model;
using System;
using System.Collections.Generic;
using System.Text.Json;
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

        private Task<DomainEntityResult> ArrangeDomainEntity()
        {
            return this.Contract.CreateEntity(new CreateDomainEntityRequest
            {
                Text = "test"
            });
        }

        public class DomainEventResponse
        {
            public DomainEventValues Event { get; set; }

            public DomainEntity Data { get; set; }
        }

        public class EventObserver : IObserver<string>
        {
            public List<string> Collected { get; } = new List<string>();

            public void OnCompleted()
            {
            }

            public void OnError(Exception error)
            {
                throw new NotImplementedException();
            }

            public void OnNext(string value) => this.Collected.Add(value);
        }

        [Fact]
        public async Task DomainService_notifies_on_create()
        {
            // ACT

            var createdEntities = new DomainEntityResult[2];

            var events = new EventObserver();

            var subscription = this.domainClient.Subscribe(events);

            var eventTask = this.domainClient.ReceiveMultipleDomainEvent();

            createdEntities[0] = await this.Contract.CreateEntity(new CreateDomainEntityRequest
            {
                Text = "test-1"
            });

            createdEntities[1] = await this.Contract.CreateEntity(new CreateDomainEntityRequest
            {
                Text = "test-2"
            });

            // ASSERT

            await eventTask;

            var resultEvent = JsonSerializer.Deserialize<DomainEventResponse>(events.Collected[0]);

            Assert.Equal(createdEntities[0].Id, resultEvent.Data.Id);

            resultEvent = JsonSerializer.Deserialize<DomainEventResponse>(events.Collected[1]);

            Assert.Equal(createdEntities[1].Id, resultEvent.Data.Id);
        }
    }
}