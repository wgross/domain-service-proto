using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace domain.contract.test
{
    public abstract class DomainServiceContractTestBase
    {
        protected IDomainService Contract { get; set; }

        protected async Task ACT_DomainService_does_something()
        {
            // ACT

            var result = await this.Contract.DoSomething(new domain.contract.DoSomethingRequest
            {
                Data = "data"
            });

            // ASSERT

            Assert.IsType<domain.contract.DoSomethingResult>(result);
        }

        protected async Task ACT_DomainService_doing_someting_fails_on_missing_body()
        {
            // ACT

            var result = await Assert.ThrowsAsync<ArgumentNullException>(() => this.Contract.DoSomething(null));

            // ASSERT

            Assert.Equal("rq", result.ParamName);
        }

        protected async Task ACT_DomainService_doing_something_fails_on_bad_input()
        {
            // ACT

            var result = await Assert.ThrowsAsync<InvalidOperationException>(() => this.Contract.DoSomething(new DoSomethingRequest
            {
                Data = null
            }));

            // ASSERT

            Assert.Equal("Data is required", result.Message);
        }

        protected async Task ACT_DomainService_creates_entity()
        {
            // ACT

            var result = await this.Contract.CreateEntity(new CreateDomainEntityRequest
            {
                Text = "test"
            });

            // ASSERT

            Assert.Equal("test", result.Text);
            Assert.NotEqual(Guid.Empty, result.Id);
        }

        protected async Task ACT_DomainService_reads_entity_by_id(Guid id)
        {
            // ACT

            var result = await this.Contract.GetEntity(id);

            // ASSERT

            Assert.Equal("test", result.Text);
            Assert.Equal(id, result.Id);
        }

        public async Task ACT_DomainService_deletes_entity_by_id(Guid entityId)
        {
            // ACT

            await this.Contract.DeleteEntity(entityId);
        }

        public async Task ACT_DomainService_reads_entities(Guid id)
        {
            // ACT

            var result = await this.Contract.GetEntities();

            // ASSERT

            Assert.Single(result.Entities);
            Assert.Equal(id, result.Entities.Single().Id);
            Assert.Equal("test", result.Entities.Single().Text);
        }

        public async Task ACT_DomainService_reading_entity_by_id_fails_on_unknown_id()
        {
            // ACT

            var result = await this.Contract.GetEntity(Guid.NewGuid());

            // ASSERT

            Assert.Null(result);
        }
    }
}