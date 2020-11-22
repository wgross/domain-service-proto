using Domain.Contract;
using Domain.Host.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Domain.Host.Controllers
{
    [Route("domain/events")]
    public sealed class DomainEventController : ControllerBase, IObserver<DomainEntityEvent>
    {
        private readonly IDomainService service;
        private readonly Channel<DomainEntityEvent> eventChannel = Channel.CreateUnbounded<DomainEntityEvent>();

        private IDisposable domainEventSubscription;
        private readonly byte[] newLineBytes;
        private readonly JsonSerializerOptions jsonSerializerOptions;

        public DomainEventController(IDomainService service)
        {
            this.newLineBytes = System.Text.Encoding.Default.GetBytes(Environment.NewLine.ToCharArray());
            this.service = service;
            this.jsonSerializerOptions = new JsonSerializerOptions
            {
                WriteIndented = false
            };
        }

        #region Push Domain Events to Web Clients

        [HttpGet()]
        public async Task Get(CancellationToken cancelled)
        {
            await BeginEventStream();

            try
            {
                using var subscription = await this.service.Subscribe(this);

                WaitHandle.WaitAll(new[] { cancelled.WaitHandle });
            }
            catch (InvalidOperationException ex)
            {
                // collection was completed
                //this.domainEventSubscription.Dispose();
                //this.domainEventSubscription = null;
            }
        }

        private async Task WriteEventStream(DomainEntityEvent currentEvent)
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

        #endregion Push Domain Events to Web Clients

        #region Receive events from DomainService

        void IObserver<DomainEntityEvent>.OnCompleted()
        {
        }

        void IObserver<DomainEntityEvent>.OnError(Exception error)
        {
        }

        void IObserver<DomainEntityEvent>.OnNext(DomainEntityEvent value) => AsyncHelper.RunSync(() => this.WriteEventStream(value));

        #endregion Receive events from DomainService
    }
}