using domain.contract;
using System;
using System.IO;
using System.Net.Http;
using System.Reactive.Subjects;
using System.Text.Json;
using System.Threading;
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

        #region Domain Command Path

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

        private Exception OnError(DomainError errorResponse)
        {
            return errorResponse.ErrorType switch
            {
                nameof(ArgumentNullException) => new ArgumentNullException(errorResponse.ParamName, errorResponse.Message),
                _ => new InvalidOperationException(errorResponse.Message)
            };
        }

        #endregion Domain Command Path

        #region Domain Query Path

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

        #endregion Domain Query Path

        #region Domain Events

        private class DomainEventReceiver : IDisposable
        {
            private readonly HttpClient httpClient;
            private readonly ISubject<DomainEntityEvent> IncomingEvents = new Subject<DomainEntityEvent>();
            private readonly CancellationTokenSource cancelListening = new CancellationTokenSource();

            public DomainEventReceiver(HttpClient httpClient)
            {
                this.httpClient = httpClient;
            }

            private async Task SubscribeAndListen(IObserver<DomainEntityEvent> observer)
            {
                // subscribe the external observer to the local subject as long as the loop is running
                using var subscription = this.IncomingEvents.Subscribe(observer);

                // await the start the response before starting the background thread
                using var response = await this.httpClient.GetAsync("/domain/events", HttpCompletionOption.ResponseHeadersRead, this.cancelListening.Token);

                // the background thread will run until the cancellation breaks it.
                await Task.Run(async () =>
                {
                    try
                    {
                        using var responseContentStream = await response.Content.ReadAsStreamAsync();
                        using var responseContentStreamReader = new StreamReader(responseContentStream);
                        do
                        {
                            var nextLineTask = responseContentStreamReader.ReadLineAsync();

                            // either await the next line of the response or the cancellation of the request
                            Task.WaitAny(new[] { nextLineTask }, cancelListening.Token);

                            // push the event to the observer
                            this.IncomingEvents.OnNext(JsonSerializer.Deserialize<DomainEntityEvent>(nextLineTask.Result));
                        }
                        while (!this.cancelListening.Token.IsCancellationRequested);
                    }
                    catch (OperationCanceledException ex)
                    {
                        return;
                    }
                    finally
                    {
                        this.httpClient.Dispose();
                    }
                });
            }

            public void Dispose()
            {
                this.cancelListening.Cancel();
            }

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            internal void Start(IObserver<DomainEntityEvent> observer) => this.SubscribeAndListen(observer);

#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        public Task<IDisposable> Subscribe(IObserver<DomainEntityEvent> observer)
        {
            var receiver = new DomainEventReceiver(this.httpClient);
            receiver.Start(observer);
            return Task.FromResult((IDisposable)receiver);
        }

        #endregion Domain Events
    }
}