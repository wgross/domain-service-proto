using Domain.Contract;
using Domain.Host.Test;
using System;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;
using Xunit;

namespace Domain.Client.PS.Test
{
    public class GetDomainEntityEventCommandTest : DomainClientCmdletTestBase
    {
        private readonly DomainServiceTestHost host;

        public GetDomainEntityEventCommandTest()
        {
            this.host = new DomainServiceTestHost();

            DomainDependencies.DomainClientFactory = _ => new DomainClient(this.host.CreateClient());
        }

        [Fact]
        public async Task GetDomainEntityEvent_writes_event_to_pipe()
        {
            // ARRANGE

            async Task<DomainEntityResult> ArrangeCreateEntity()
            {
                using var client = new DomainClient(this.host.CreateClient());
                return await client.CreateEntity(new CreateDomainEntityRequest
                {
                    Text = "text-1"
                });
            }

            // ACT

            PSObject[] result = default;

            var actTask = Task.Run(() =>
            {
                result = this.PowerShell.AddCommand("Get-DomainEntityEvent").Invoke().ToArray();
            });

            var arrangedEntity = await ArrangeCreateEntity();

            this.PowerShell.Stop();

            await actTask;

            // ASSERT

            Assert.IsType<DomainEntityEvent>(result.Single().ImmediateBaseObject);
            Assert.Equal(arrangedEntity.Id, (Guid)result.Single().Properties[nameof(DomainEntityEvent.Id)].Value);
            Assert.Equal(DomainEntityEventTypes.Added, (DomainEntityEventTypes)result.Single().Properties[nameof(DomainEntityEvent.EventType)].Value);
        }
    }
}