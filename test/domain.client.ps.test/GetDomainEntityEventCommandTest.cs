using Domain.Contract;
using System;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;
using Xunit;

namespace Domain.Client.PS.Test
{
    public class GetDomainEntityEventCommandTest : DomainClientCmdletTestBase
    {
        [Fact]
        public async Task GetDomainEntityEvent_writes_event_to_pipe()
        {
            // ARRANGE

            this.ArrangeToken();

            async Task<DomainEntityResult> ArrangeDomainEntity()
            {
                return await DomainClientSession.CurrentDomainClientFactory(this.Host.Server.BaseAddress).CreateEntity(new CreateDomainEntityRequest
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

            var arrangedEntity = await ArrangeDomainEntity();

            this.PowerShell.Stop();

            await actTask;

            // ASSERT

            Assert.IsType<DomainEntityEvent>(result.Single().ImmediateBaseObject);
            Assert.Equal(arrangedEntity.Id, (Guid)result.Single().Properties[nameof(DomainEntityEvent.Id)].Value);
            Assert.Equal(DomainEntityEventTypes.Added, (DomainEntityEventTypes)result.Single().Properties[nameof(DomainEntityEvent.EventType)].Value);
        }
    }
}