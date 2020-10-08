using domain.client;
using domain.contract.test;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Threading.Tasks;
using Xunit;

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

        [Fact]
        public Task DomainService_does_something() => base.ACT_DomainService_does_something();

        [Fact]
        public Task DomainService_doing_someting_fails_on_missing_body() => base.ACT_DomainService_doing_someting_fails_on_missing_body();

        [Fact]
        public Task DomainService_doing_something_fails_on_bad_input() => base.ACT_DomainService_doing_something_fails_on_bad_input();

        [Fact]
        public async Task DomainService_creates_entity() => base.ACT_DomainService_creates_entity();
    }
}