using System.Threading.Tasks;
using Xunit;

namespace domain.service.test
{
    public class DomainServiceDoSomethingTest
    {
        private readonly DomainService domainService;

        public DomainServiceDoSomethingTest()
        {
            this.domainService = new DomainService();
        }

        [Fact]
        public async Task DomainService_does_something()
        {
            // ACT

            var result = await this.domainService.DoSomething(new contract.DoSomethingRequest());

            // ASSERT

            Assert.NotNull(result);
        }
    }
}