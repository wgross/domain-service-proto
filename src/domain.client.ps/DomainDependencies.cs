using System;
using System.Net.Http;

namespace Domain.Client.PS
{
    public static class DomainDependencies
    {
        public static Func<Uri, DomainClient> DomainClientFactory { get; set; } = baseAddress => new DomainClient(new HttpClient
        {
            BaseAddress = baseAddress
        });
    }
}