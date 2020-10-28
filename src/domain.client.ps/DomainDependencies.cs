using System;
using System.Net.Http;

namespace domain.client.ps
{
    public static class DomainDependencies
    {
        public static Func<Uri, DomainClient> DomainClientFactory { get; set; } = baseAddress => new DomainClient(new HttpClient
        {
            BaseAddress = baseAddress
        });
    }
}