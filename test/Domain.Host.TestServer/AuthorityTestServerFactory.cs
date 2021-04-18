using IdentityServer4.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Domain.Host.TestServer
{
    public sealed class AuthorityTestServerFactory
    {
        public AuthorityTestServerFactory()
        {
            this.Server = Create();
        }

        public Microsoft.AspNetCore.TestHost.TestServer Server { get; }

        #region Configure the test Authority

        public AuthorityTestServerConfiguration Configuration { get; } = new AuthorityTestServerConfiguration();

        public void AddClientCredentials(string clientId, string clientSecret, params string[] scopes)
        {
            this.Configuration.Clients.Add(new Client
            {
                ClientId = "client",
                AllowedScopes = scopes,
                ClientSecrets = new[]
                {
                    new Secret
                    {
                        Value = clientSecret.Sha256()
                    }
                },
                AllowedGrantTypes = GrantTypes.ClientCredentials
            });

            foreach (var scope in scopes)
            {
                this.Configuration.ApiResources.Add(new ApiResource
                {
                    Name = scope,
                });
            }

            foreach (var scope in scopes)
            {
                this.Configuration.ApiScopes.Add(new ApiScope
                {
                    Name = scope,
                });
            }
        }

        #endregion Configure the test Authority

        private Microsoft.AspNetCore.TestHost.TestServer Create()
        {
            var hostBuilder = Microsoft.AspNetCore.WebHost
                  .CreateDefaultBuilder()
                  .ConfigureServices(services => services
                      .AddIdentityServer()
                      .AddInMemoryApiResources(this.Configuration.ApiResources)
                      .AddInMemoryApiScopes(this.Configuration.ApiScopes)
                      .AddInMemoryClients(this.Configuration.Clients)
                      .AddDeveloperSigningCredential(persistKey: false))
                  .Configure(app => app.UseIdentityServer());

            return new Microsoft.AspNetCore.TestHost.TestServer(hostBuilder);
        }
    }
}