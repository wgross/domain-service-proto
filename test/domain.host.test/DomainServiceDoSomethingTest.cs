using domain.client;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Threading.Tasks;
using Xunit;

namespace host.test
{
    public class DomainServiceDoSomethingTest
    {
        private readonly WebApplicationFactory<Startup> factory;
        private readonly DomainClient client;

        public DomainServiceDoSomethingTest()
        {
            this.factory = new WebApplicationFactory<host.Startup>();
            this.client = new DomainClient(this.factory.CreateClient());
        }

        [Fact]
        public async Task DomainService_does_something()
        {
            // ACT

            var result = await this.client.DoSomething(new domain.contract.DoSomethingRequest());

            // ASSERT

            Assert.IsType<domain.contract.DoSomethingResult>(result);
        }
    }
}