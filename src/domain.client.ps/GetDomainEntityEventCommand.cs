using Domain.Contract;
using Domain.Host.Controllers;
using System;
using System.Management.Automation;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Domain.Client.PS
{
    [Cmdlet(VerbsCommon.Get, nameof(DomainEntityEvent))]
    public sealed class GetDomainEntityEventCommand : PSCmdlet
    {
        [Parameter]
        [ValidateNotNull()]
        public Uri DomainService { get; set; } = new Uri("http://localhost:6000");

        private class DomainEventObserver : IObserver<DomainEntityEvent>
        {
            private readonly ChannelWriter<DomainEntityEvent> domainEventWriter;

            public DomainEventObserver(ChannelWriter<DomainEntityEvent> eventQ)
            {
                this.domainEventWriter = eventQ;
            }

            public void OnCompleted() => this.domainEventWriter.Complete();

            public void OnError(Exception error)
            { }

            public void OnNext(DomainEntityEvent value)
            {
                AsyncHelper.RunSync(() => this.domainEventWriter.WriteAsync(value).AsTask());
            }
        }

        private readonly CancellationTokenSource waitForEvents = new CancellationTokenSource();
        private readonly Channel<DomainEntityEvent> domainEventsChannel = Channel.CreateUnbounded<DomainEntityEvent>();

        protected override void ProcessRecord()
        {
            // create aclient and start listening to events
            using var domainClient = DomainClientSession.CurrentDomainClientFactory(this.DomainService);
            using var subscription = AsyncHelper.RunSync(() => domainClient.Subscribe(new DomainEventObserver(this.domainEventsChannel.Writer)));

            do
            {
                var listeningTask = this.domainEventsChannel.Reader.ReadAsync().AsTask();

                // block until an event is passed or the cancellation was requested
                Task.WaitAny(new[] { listeningTask }, this.waitForEvents.Token);

                if (!this.waitForEvents.IsCancellationRequested)
                    this.WriteObject(AsyncHelper.RunSync(() => listeningTask));
            }
            while (!this.waitForEvents.IsCancellationRequested);
        }

        protected override void StopProcessing()
        {
            this.waitForEvents.Cancel();
            this.domainEventsChannel.Writer.Complete();
        }
    }
}