using System;
using System.Net.Http;

namespace Domain.Client.PS
{
    public static class DomainDependencies
    {
        public static Func<Uri, JsonDomainClient> DomainClientFactory { get; set; } =
            baseAddress => new JsonDomainClient(new HttpClient(), new JsonDomainClientOptions { DomainService = baseAddress });
    }
}