using domain.model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace domain.host.controllers
{
    [Route("domain/events")]
    public sealed class DomainEventController : ControllerBase, IObserver<DomainEvent>
    {
        private readonly IDomainModel model;
        private readonly BlockingCollection<DomainEvent> eventQueue = new BlockingCollection<DomainEvent>();

        private IDisposable domainEventSubscription;
        private readonly JsonSerializerOptions jsonSerializerOptions;

        public DomainEventController(IDomainModel model)
        {
            this.model = model;
            this.domainEventSubscription = model.DomainEvents.Subscribe(this);
            this.jsonSerializerOptions = new JsonSerializerOptions
            {
                WriteIndented = false
            };
        }

        [HttpGet()]
        public async Task Get(CancellationToken cancelled)
        {
            this.HttpContext.Response.ContentType = "text/event-stream";
            await this.HttpContext.Response.Body.FlushAsync();

            try
            {
                do
                {
                    var currentEvent = this.eventQueue.Take(cancelled);

                    await JsonSerializer.SerializeAsync(this.HttpContext.Response.Body, currentEvent, this.jsonSerializerOptions);
                    await this.HttpContext.Response.Body.FlushAsync();
                }
                while (!cancelled.IsCancellationRequested);
            }
            catch (InvalidOperationException)
            {
                // collection was completed
                this.domainEventSubscription.Dispose();
                this.domainEventSubscription = null;
            }
        }

        public void OnCompleted() => this.eventQueue.CompleteAdding();

        public void OnError(Exception error)
        {
        }

        public void OnNext(DomainEvent value) => this.eventQueue.Add(value);
    }
}