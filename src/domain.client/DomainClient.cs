using domain.contract;
using System;
using System.IO;
using System.Net.Http;
using System.Reactive.Subjects;
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

        #region CRUD entities

        public async Task<DomainEntityResult> CreateEntity(CreateDomainEntityRequest createDomainEntity)
        {
            var response = await this.httpClient.PostAsJsonAsync("/domain", createDomainEntity);

            return await response.Content.ReadAsAsync<domain.contract.DomainEntityResult>();
        }

        public Task DeleteEntity(Guid entityId)
        {
            return this.httpClient.DeleteAsync($"/domain/{entityId}");
        }

        public async Task<DoSomethingResult> DoSomething(DoSomethingRequest rq)
        {
            var response = await this.httpClient.PostAsJsonAsync("/domain/do", rq);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadAsAsync<domain.contract.DoSomethingResult>();
            else throw OnError(await response.Content.ReadAsAsync<DomainError>());
        }

        public async Task<DomainEntityCollectionResult> GetEntities()
        {
            var response = await this.httpClient.GetAsync($"/domain");

            return await response.Content.ReadAsAsync<domain.contract.DomainEntityCollectionResult>();
        }

        public async Task<DomainEntityResult> GetEntity(Guid id)
        {
            var response = await this.httpClient.GetAsync($"/domain/{id}");

            return await response.Content.ReadAsAsync<domain.contract.DomainEntityResult>();
        }

        private Exception OnError(DomainError errorResponse)
        {
            return errorResponse.ErrorType switch
            {
                nameof(ArgumentNullException) => new ArgumentNullException(errorResponse.ParamName, errorResponse.Message),
                _ => new InvalidOperationException(errorResponse.Message)
            };
        }

        #endregion CRUD entities

        #region Notify on entities

        private readonly ISubject<string> IncomingEvents = new Subject<string>();

        public IDisposable Subscribe(IObserver<string> eventObserver) => this.IncomingEvents.Subscribe(eventObserver);

        private void Publish(string ev) => this.IncomingEvents.OnNext(ev);

        public async Task<string> ReceiveSingleDomainEvent()
        {
            var resultStream = await this.httpClient.GetStreamAsync("/domain/events");
            using var resultReader = new StreamReader(resultStream);
            return await resultReader.ReadLineAsync();
        }

        public Task ReceiveMultipleDomainEvent()
        {
            return Task.Run(async () =>
            {
                var resultMessage = await this.httpClient.GetAsync("/domain/events", HttpCompletionOption.ResponseHeadersRead);
                using var resultStream = await resultMessage.Content.ReadAsStreamAsync();
                using var resultReader = new StreamReader(resultStream);

                Publish(await resultReader.ReadLineAsync());
                Publish(await resultReader.ReadLineAsync());
            });
        }

        #endregion Notify on entities
    }
}