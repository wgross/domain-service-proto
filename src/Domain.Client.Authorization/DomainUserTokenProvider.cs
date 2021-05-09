using IdentityModel.Client;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Domain.Client.Authorization
{
    public class DomainUserTokenProvider : IDomainUserTokenProvider
    {
        private readonly Uri authority;
        private readonly HttpClient authorityHttpClient;
        private TokenResponse userCredentials;

        public DomainUserTokenProvider(Uri authority, HttpClient authorityHttpClient)
        {
            this.authority = authority;
            this.authorityHttpClient = authorityHttpClient;
        }

        public async Task FetchToken(string clientId, string clientSecret, string userId, string userSecret, string[] scopes)
        {
            var discoveryDocument = await this.authorityHttpClient.GetDiscoveryDocumentAsync(this.authority?.AbsoluteUri);
            this.userCredentials = await this.authorityHttpClient.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = discoveryDocument.TokenEndpoint,
                ClientId = clientId,
                ClientSecret = clientSecret,
                UserName = userId,
                Password = userSecret,
                Scope = string.Join(" ", scopes)
            });
            if (this.userCredentials.IsError)
            {
                throw new InvalidOperationException($"Fetching user credentials failed: {userCredentials.Error}");
            }
        }

        public string GetAccessToken() => this.userCredentials.AccessToken;
    }
}