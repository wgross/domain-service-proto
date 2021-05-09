using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Client.Authorization
{
    /// <summary>
    /// Uses the <see cref="ITokenProvider"/> to add a bearer token to the outgoing request.
    /// This allows the code of <see cref="JsonDomainClient"/> to be free of the clutter of authorization code
    /// </summary>
    public class DomainClientAuthorizingHandler : DelegatingHandler
    {
        private readonly ITokenProvider tokenProvider;

        public DomainClientAuthorizingHandler(ITokenProvider tokenProvider) : base()
        {
            this.tokenProvider = tokenProvider;
        }

        public DomainClientAuthorizingHandler(HttpMessageHandler innerHandler, IDomainClientTokenProvider tokenProvider) : base(innerHandler)
        {
            this.tokenProvider = tokenProvider;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Add bearer token if avaliable
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", this.tokenProvider.GetAccessToken());

            var response = await base.SendAsync(request, cancellationToken);

            // and handle map an unauthorized response to an exception
            if (!response.IsSuccessStatusCode && response.StatusCode == HttpStatusCode.Unauthorized)
                throw new InvalidOperationException(response.ReasonPhrase);

            return response;
        }
    }
}