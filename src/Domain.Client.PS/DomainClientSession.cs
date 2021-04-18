using Domain.Client.Authorization;
using Domain.Client.Json;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;

namespace Domain.Client.PS
{
    public class DomainClientSessionOptions
    {
        public Uri Authority { get; set; }
    }

    public class DomainClientSession
    {
        /// <summary>
        /// Setup the powershell client state to be abvle to access the domain service.
        /// </summary>
        /// <param name="setup"></param>
        public static void Setup(Action<DomainClientSessionOptions> setup)
        {
            var tmp = new DomainClientSessionOptions();
            setup.Invoke(tmp);

            CurrentTokenProvider = new DomainClientTokenProvider(
                authority: tmp.Authority,
                authorityHttpClient: AuthorityClientOverride ?? new HttpClient());

            CurrentDomainClientFactory = baseAddress => JsonDomainClient.Create(
                options: Options.Create(new JsonDomainClientOptions
                {
                    DomainService = baseAddress
                }),
                tokenProvider: CurrentTokenProvider,
                domainClientHandler: DomainClientHandler);
        }

        public static HttpClient AuthorityClientOverride { internal get; set; }

        public static HttpMessageHandler DomainClientHandler { internal get; set; }

        public static IDomainClientTokenProvider CurrentTokenProvider { get; private set; }

        public static Func<Uri, JsonDomainClient> CurrentDomainClientFactory { get; set; }
    }
}