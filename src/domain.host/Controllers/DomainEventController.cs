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
        private readonly byte[] newLineBytes;
        private readonly JsonSerializerOptions jsonSerializerOptions;

        public DomainEventController(IDomainModel model)
        {
            this.newLineBytes = System.Text.Encoding.Default.GetBytes(Environment.NewLine.ToCharArray());
            this.model = model;
            this.domainEventSubscription = model.DomainEvents.Subscribe(this);
            this.jsonSerializerOptions = new JsonSerializerOptions
            {
                WriteIndented = false
            };
        }

        #region Push Domain events to Web clients

        [HttpGet()]
        public async Task Get(CancellationToken cancelled)
        {
            await BeginEventStream();

            try
            {
                do
                {
                    await foreach (var currentEvent in this.eventChannel.Reader.ReadAllAsync(cancelled))
                    {
                        await WriteEventStream(currentEvent);
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

        private async Task WriteEventStream(DomainEvent currentEvent)
        {
            await JsonSerializer.SerializeAsync(this.HttpContext.Response.Body, currentEvent, this.jsonSerializerOptions);
            await this.HttpContext.Response.Body.WriteAsync(this.newLineBytes, 0, this.newLineBytes.Length);
            await this.HttpContext.Response.Body.FlushAsync();
        }

        private async Task BeginEventStream()
        {
            this.HttpContext.Response.ContentType = "text/event-stream";
            await this.HttpContext.Response.Body.FlushAsync();
        }

        #endregion Push Domain events to Web clients

        #region Receive Domain Events

        public void OnCompleted() => this.eventChannel.Writer.Complete();

        public void OnError(Exception error)
        {
        }

        public void OnNext(DomainEvent value) => this.eventChannel.Writer.TryWrite(value);

        #endregion Receive Domain Events
    }
}