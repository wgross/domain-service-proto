using IdentityServer4.Models;
using System.Collections.Generic;

namespace Domain.Host.TestServer
{
    public class AuthorityTestServerConfiguration
    {
        public ICollection<ApiResource> ApiResources { get; } = new List<ApiResource>();

        public ICollection<ApiScope> ApiScopes { get; } = new List<ApiScope>();

        public ICollection<IdentityServer4.Models.Client> Clients { get; } = new List<Client>();
    }
}