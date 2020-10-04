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

            var result = await this.Contract.DoSomething(new domain.contract.DoSomethingRequest());

            // ASSERT

            Assert.IsType<domain.contract.DoSomethingResult>(result);
        }
    }
}