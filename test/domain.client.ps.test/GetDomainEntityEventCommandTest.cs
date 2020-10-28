using domain.contract;
using domain.host.test;
using System.Threading.Tasks;
using Xunit;

namespace domain.client.ps.test
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
                return await client.CreateEntity(new contract.CreateDomainEntityRequest
                {
                    Text = "text-1"
                });
            }

            // ACT

            var actTask = Task.Run(() =>
            {
                var result = this.PowerShell.AddCommand("Get-DomainEntityEvent").Invoke();
            });

            await ArrangeCreateEntity();

            this.PowerShell.Stop();

            await actTask;
        }
    }
}