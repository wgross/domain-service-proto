using Domain.Client.Authorization;
using Domain.Client.Json;
using Domain.Host.TestServer;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Domain.Host.Test
{
    public abstract class DomainServiceIntegTestBase
    {
        protected AuthorityTestServerFactory Authority { get; private set; }

        protected DomainHostTestServer Host { get; }
        protected IDomainClientTokenProvider TokenProvider { get; private set; }

        
        public DomainServiceIntegTestBase()
        {
            this.ArrangeAuthority();
            this.ArrangeNewTokenProvider();

            this.Host = new DomainHostTestServer(this.Authority.Server);

            
        }

        #region Arrange Test Environment

        protected void ArrangeNewTokenProvider() => this.TokenProvider = new DomainClientTokenProvider(this.Authority.Server.BaseAddress, this.Authority.Server.CreateClient());

        protected Task ArrangeToken() => this.TokenProvider.FetchToken("client", "password123$", new[] { "DomainService" });

        protected async Task ArrangeInvalidToken()
        {
            try
            {
                await this.TokenProvider.FetchToken("client", "-wrong-", new[] { "DomainService" });
            }
            catch
            {
            }
        }

        public void ArrangeAuthority()
        {
            var authority = new AuthorityTestServerFactory();
            authority.AddClientCredentials("client", "password123$", "DomainService");
            this.Authority = authority;
        }

        #endregion Arrange Test Environment

        #region Assert test results

        protected void AssertUnauthorized(InvalidOperationException result)
        {
            Assert.IsType<InvalidOperationException>(result);
        }

        #endregion Assert test results
    }
}