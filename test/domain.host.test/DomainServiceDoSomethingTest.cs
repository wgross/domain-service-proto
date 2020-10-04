using domain.client;
using domain.contract.test;
using Microsoft.AspNetCore.Mvc.Testing;

namespace host.test
{
    public class DomainServiceDoSomethingTest : DomainServiceDoSomethingTestBase
    {
        private readonly WebApplicationFactory<Startup> factory;

        public DomainServiceDoSomethingTest()
        {
            this.factory = new WebApplicationFactory<host.Startup>();
            this.Contract = new DomainClient(this.factory.CreateClient());
        }
    }
}