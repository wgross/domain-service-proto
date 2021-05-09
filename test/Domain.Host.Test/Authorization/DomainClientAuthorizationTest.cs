using Domain.Client.Authorization;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Domain.Host.Test.Authorization
{
    public class DomainClientAuthorizationTest : DomainServiceIntegTestBase
    {
        private readonly DomainClientTokenProvider tokenProvider;

        public DomainClientAuthorizationTest()
        {
            this.tokenProvider = new DomainClientTokenProvider(this.Authority.Server.BaseAddress, this.Authority.Server.CreateClient());
        }

        [Fact]
        public async Task TokenProvider_fetches_token_from_Authority()
        {
            // ACT

            await this.tokenProvider.FetchToken("client", "password123$", new[] { "DomainService" });

            // ASSERT

            Assert.False(string.IsNullOrEmpty(this.tokenProvider.GetAccessToken()));
        }

        [Fact]
        public async Task TokenProvider_fetching_token_fails_in_wrong_secret()
        {
            // ACT

            var result = await Assert.ThrowsAsync<InvalidOperationException>(() => this.tokenProvider.FetchToken("client", "-wrong-", new[] { "DomainService" }));

            // ASSERT

            Assert.Equal("Fetching client credentials failed: invalid_client", result.Message);
        }

        [Fact]
        public void TokenProvider_raeding_token_before_fetching_fails()
        {
            // ACT

            var result = Assert.Throws<InvalidOperationException>(() => this.tokenProvider.GetAccessToken());

            // ASSERT

            Assert.Equal("Token hasn't been fetched", result.Message);
        }
    }
}