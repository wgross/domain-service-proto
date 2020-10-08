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

        public Task<DomainEntityResult> CreateEntity(CreateDomainEntityRequest createDomainEntity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteEntity(Guid entityId)
        {
            throw new NotImplementedException();
        }

        public async Task<DoSomethingResult> DoSomething(DoSomethingRequest rq)
        {
            var response = await this.httpClient.PostAsJsonAsync("/domain/do", rq);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadAsAsync<domain.contract.DoSomethingResult>();
            else throw OnError(await response.Content.ReadAsAsync<DomainError>());
        }

        public Task<DomainEntityCollectionResult> GetEntities()
        {
            throw new NotImplementedException();
        }

        public Task<DomainEntityResult> GetEntity(Guid id)
        {
            throw new NotImplementedException();
        }

        private Exception OnError(DomainError errorResponse)
        {
            return errorResponse.ErrorType switch
            {
                nameof(ArgumentNullException) => new ArgumentNullException(errorResponse.ParamName, errorResponse.Message),
                _ => new InvalidOperationException(errorResponse.Message)
            };
        }
    }
}