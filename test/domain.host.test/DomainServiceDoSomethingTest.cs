using domain.client;
using domain.contract;
using domain.contract.test;
using domain.host.test;
using System.Threading.Tasks;
using Xunit;

namespace host.test
{
    public class DomainServiceDoSomethingTest : DomainServiceDoSomethingTestBase
    {
        private readonly DomainServiceTestHost host;

        public DomainServiceDoSomethingTest()
        {
            this.host = new DomainServiceTestHost();
            this.Contract = new DomainClient(this.host.CreateClient());
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
    }
}