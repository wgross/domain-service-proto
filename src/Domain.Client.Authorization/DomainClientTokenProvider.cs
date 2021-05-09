using IdentityModel.Client;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Domain.Client.Authorization
{
    /// <summary>
    /// Fetches a new token from the given authority and stores it for reuse.
    /// </summary>
    public sealed class DomainClientTokenProvider : IDomainClientTokenProvider
    {
        private readonly Uri authority;
        private readonly HttpClient authorityHttpClient;
        private TokenResponse clientCredentials;

        public DomainClientTokenProvider(Uri authority)
            : this(authority, new HttpClient())
        { }

        public DomainClientTokenProvider(Uri authority, HttpClient authorityHttpClient)
        {
            this.authority = authority;
            this.authorityHttpClient = authorityHttpClient;
        }

        public async Task FetchToken(string clientId, string clientSecret, string[] scopes)
        {
            var discoveryDocument = await this.authorityHttpClient.GetDiscoveryDocumentAsync(this.authority?.AbsoluteUri);
            this.clientCredentials = await this.authorityHttpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = discoveryDocument.TokenEndpoint,
                ClientId = clientId,
                ClientSecret = clientSecret,
                Scope = string.Join(" ", scopes)
            });
            if (this.clientCredentials.IsError)
            {
                throw new InvalidOperationException($"Fetching client credentials failed: {clientCredentials.Error}");
            }
        }

        public string GetAccessToken() => this.clientCredentials?.AccessToken ?? throw new InvalidOperationException("Token hasn't been fetched");
    }
}