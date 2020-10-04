using domain.contract;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace domain.client
{
    public sealed class DomainClient : IDomainService
    {
        private readonly HttpClient httpClient;

        public DomainClient(HttpClient client)
        {
            this.httpClient = client;
        }

        public async Task<DoSomethingResult> DoSomething(DoSomethingRequest rq)
        {
            if (rq is null)
                throw new ArgumentNullException(nameof(rq));

            var response = await this.httpClient.PostAsJsonAsync("/domain", rq);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadAsAsync<domain.contract.DoSomethingResult>();
            else throw OnError(await response.Content.ReadAsAsync<DomainError>());
        }

        private Exception OnError(DomainError errorResponse) => new InvalidOperationException(errorResponse.Reason);
    }
}