using System;
using System.Threading.Tasks;
using Xunit;

namespace domain.contract.test
{
    public abstract class DomainServiceDoSomethingTestBase
    {
        protected IDomainService Contract { get; set; }

        [Fact]
        public async Task DomainService_does_something()
        {
            // ACT

            var result = await this.Contract.DoSomething(new domain.contract.DoSomethingRequest
            {
                Data = "data"
            });

            // ASSERT

            Assert.IsType<domain.contract.DoSomethingResult>(result);
        }

        [Fact]
        public async Task DomainService_doing_someting_fails_on_missing_body()
        {
            // ACT

            var result = await Assert.ThrowsAsync<ArgumentNullException>(() => this.Contract.DoSomething(null));

            // ASSERT

            Assert.Equal("rq", result.ParamName);
        }

        [Fact]
        public async Task DomainService_doing_something_fails_on_bad_input()
        {
            // ACT

            var result = await Assert.ThrowsAsync<InvalidOperationException>(() => this.Contract.DoSomething(new DoSomethingRequest
            {
                Data = null
            }));

            // ASSERT

            Assert.Equal("Data is required", result.Message);
        }
    }
}