using Domain.Client.Authorization;
using Domain.Contract;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reactive.Subjects;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Client.Json
{
    public sealed class JsonDomainClient : IDomainService, IDisposable
    {
        private HttpClient domainHttpClient;

        public HttpClient HttpClient => this.domainHttpClient;

        public JsonDomainClient(HttpClient httpClient, IOptions<JsonDomainClientOptions> options)
        {
            this.domainHttpClient = httpClient;
            this.domainHttpClient.BaseAddress = options.Value.DomainService;
        }

        public void Dispose()
        {
            // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-2.1#httpclient-and-lifetime-management
            // HttpClient instances may be disposed but should be treated as non-diposable in general
            this.domainHttpClient = null;
        }

        /// <summary>
        /// Creates an instance of the domains HTTP+JSON client.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="tokenProvider">optional: source of the client authentication token</param>
        /// <param name="domainClientHandler">optional: alternative <see cref="HttpClientHandler"/> for test scenarios for example</param>
        /// <returns></returns>
        public static JsonDomainClient Create(IOptions<JsonDomainClientOptions> options, IDomainClientTokenProvider tokenProvider = null, HttpMessageHandler domainClientHandler = null)
            => new JsonDomainClient(CreateHttpClient(tokenProvider, domainClientHandler), options);

        private static HttpClient CreateHttpClient(IDomainClientTokenProvider tokenProvider, HttpMessageHandler domainClientHandler)
        {
            if (tokenProvider is null)
            {
                if (domainClientHandler is null)
                {
                    return new HttpClient();
                }
                else return new HttpClient(domainClientHandler);
            }
            else
            {
                if (domainClientHandler is null)
                {
                    return new HttpClient(new DomainClientAuthorizingHandler(tokenProvider));
                }
                else return new HttpClient(new DomainClientAuthorizingHandler(tokenProvider)
                {
                    InnerHandler = domainClientHandler
                });
            }
        }

        #region Domain Command Path

        /// <inheritdoc/>
        public async Task<DomainEntityResult> CreateEntity(CreateDomainEntityRequest createDomainEntity)
        {
            var response = await this.domainHttpClient.PostAsJsonAsync("/domain", createDomainEntity).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadAsAsync<Domain.Contract.DomainEntityResult>().ConfigureAwait(false);
            else throw await OnError(response);
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteEntity(Guid entityId)
        {
            var response = await this.domainHttpClient.DeleteAsync($"/domain/{entityId}").ConfigureAwait(false);
            return response switch
            {
                HttpResponseMessage r when r.IsSuccessStatusCode => true,
                HttpResponseMessage r when r.StatusCode == System.Net.HttpStatusCode.NotFound => false,

                _ => throw new InvalidOperationException($"Unknown error: {response.StatusCode}")
            };
        }

        /// <inheritdoc/>
        public async Task<DomainEntityResult> UpdateEntity(Guid entityId, UpdateDomainEntityRequest updateDomainEntity)
        {
            var response = await this.domainHttpClient.PutAsJsonAsync($"/domain/{entityId}", updateDomainEntity).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadAsAsync<Domain.Contract.DomainEntityResult>();
            else throw await OnError(response);
        }

        /// <inheritdoc/>
        public async Task<DoSomethingResult> DoSomething(DoSomethingRequest rq)
        {
            var response = await this.domainHttpClient.PostAsJsonAsync("/domain/do", rq).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadAsAsync<Domain.Contract.DoSomethingResult>();
            else throw await OnError(response);
        }

        private async Task<Exception> OnError(HttpResponseMessage errorResponse)
        {
            if (errorResponse.StatusCode == HttpStatusCode.InternalServerError)
                return new InvalidOperationException(await errorResponse.Content.ReadAsStringAsync().ConfigureAwait(false));

            return this.OnError(await errorResponse.Content.ReadAsAsync<DomainError>().ConfigureAwait(false));
        }

        private Exception OnError(DomainError errorResponse)
        {
            // map names of well known exceptions to new exception instances.
            return errorResponse.ErrorType switch
            {
                nameof(ArgumentNullException) => new ArgumentNullException(errorResponse.ParamName, errorResponse.Message),
                nameof(DomainEntityMissingException) => new DomainEntityMissingException(errorResponse.Message),
                _ => new InvalidOperationException(errorResponse.Message)
            };
        }

        #endregion Domain Command Path

        #region Domain Query Path

        /// <inheritdoc/>
        public async Task<DomainEntityCollectionResult> GetEntities()
        {
            var response = await this.domainHttpClient.GetAsync($"/domain");

            return await response.Content.ReadAsAsync<Domain.Contract.DomainEntityCollectionResult>();
        }

        /// <inheritdoc/>
        public async Task<DomainEntityResult> GetEntity(Guid id)
        {
            var response = await this.domainHttpClient.GetAsync($"/domain/{id}");

            return await response.Content.ReadAsAsync<Domain.Contract.DomainEntityResult>();
        }

        #endregion Domain Query Path

        #region Domain Events

        private class DomainEventReceiver : IDisposable
        {
            private HttpClient httpClient;
            private readonly ISubject<DomainEntityEvent> IncomingEvents = new Subject<DomainEntityEvent>();
            private CancellationTokenSource cancelListening = new CancellationTokenSource();

            public DomainEventReceiver(HttpClient httpClient)
            {
                this.httpClient = httpClient;
            }

            private async Task SubscribeAndListen(IObserver<DomainEntityEvent> observer)
            {
                // the background thread will run until the cancellation breaks it.
                await Task
                    .Run(async () =>
                    {
                        // subscribe the external observer to the local subject as long as the loop is running
                        using var subscription = this.IncomingEvents.Subscribe(observer);

                        try
                        {
                            // await the start the response before starting the background thread
                            using var response = await this.httpClient
                            .GetAsync("/domain/events", HttpCompletionOption.ResponseHeadersRead, this.cancelListening.Token)
                            .ConfigureAwait(false);

                            // wait for incoming data
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
                        catch (OperationCanceledException)
                        {
                            return;
                        }
                    })
                    .ConfigureAwait(false);
            }

            public void Dispose()
            {
                this.cancelListening?.Cancel();
                this.cancelListening = null;
                // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-2.1#httpclient-and-lifetime-management
                // HttpClient instances may be disposes but should be treated as non-diposable in general
                this.httpClient = null;
            }

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            internal void Start(IObserver<DomainEntityEvent> observer) => this.SubscribeAndListen(observer);

#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        /// <inheritdoc/>
        public Task<IDisposable> Subscribe(IObserver<DomainEntityEvent> observer)
        {
            var receiver = new DomainEventReceiver(this.domainHttpClient);
            receiver.Start(observer);
            return Task.FromResult((IDisposable)receiver);
        }

        #endregion Domain Events
    }
}