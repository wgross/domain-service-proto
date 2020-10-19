using domain.model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace domain.host.controllers
{
    [Route("domain/events")]
    public sealed class DomainEventController : ControllerBase, IObserver<DomainEvent>
    {
        private readonly IDomainModel model;
        private readonly Channel<DomainEvent> eventChannel = Channel.CreateUnbounded<DomainEvent>();

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

            var newLineArray = System.Text.Encoding.Default.GetBytes(Environment.NewLine.ToCharArray());

            try
            {
                do
                {
                    await foreach (var currentEvent in this.eventChannel.Reader.ReadAllAsync(cancelled))
                    {
                        await JsonSerializer.SerializeAsync(this.HttpContext.Response.Body, currentEvent, this.jsonSerializerOptions);
                        await this.HttpContext.Response.Body.WriteAsync(newLineArray, 0, newLineArray.Length);
                        await this.HttpContext.Response.Body.FlushAsync();
                    }
                }
                while (!cancelled.IsCancellationRequested);
            }
            catch (InvalidOperationException ex)
            {
                // collection was completed
                this.domainEventSubscription.Dispose();
                this.domainEventSubscription = null;
            }
        }

        public void OnCompleted() => this.eventChannel.Writer.Complete();

        public void OnError(Exception error)
        {
        }

        public void OnNext(DomainEvent value) => this.eventChannel.Writer.TryWrite(value);
    }
}