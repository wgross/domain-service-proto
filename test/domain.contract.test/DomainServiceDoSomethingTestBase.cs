using System;
using System.Threading.Tasks;
using Xunit;

namespace domain.contract.test
{
    public abstract class DomainServiceDoSomethingTestBase
    {
        protected IDomainService Contract { get; set; }

        protected async Task DomainService_does_something_Act()
        {
            // ACT

            var result = await this.Contract.DoSomething(new domain.contract.DoSomethingRequest
            {
                Data = "data"
            });

            // ASSERT

            Assert.IsType<domain.contract.DoSomethingResult>(result);
        }

        protected async Task DomainService_doing_someting_fails_on_missing_body_Act()
        {
            // ACT

            var result = await Assert.ThrowsAsync<ArgumentNullException>(() => this.Contract.DoSomething(null));

            // ASSERT

            Assert.Equal("rq", result.ParamName);
        }

        protected async Task DomainService_doing_something_fails_on_bad_input_Act()
        {
            // ACT

            var result = await Assert.ThrowsAsync<InvalidOperationException>(() => this.Contract.DoSomething(new DoSomethingRequest
            {
                Data = null
            }));

            // ASSERT

            Assert.Equal("Data is required", result.Message);
        }

        protected async Task DomainService_creates_entity_Act()
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

        protected async Task DomainService_reads_entity_by_id_Act(Guid id)
        {
            // ACT

            var result = await this.Contract.GetEntity(id);

            // ASSERT

            Assert.Equal("test", result.Text);
            Assert.Equal(id, result.Id);
        }
    }
}