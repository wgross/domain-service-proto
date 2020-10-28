using Moq;
using System.Management.Automation;
using Xunit;

namespace domain.client.ps.test
{
    [Collection("UsesPowershell")]
    public abstract class DomainClientCmdletTestBase
    {
        protected MockRepository Mocks { get; } = new MockRepository(MockBehavior.Strict);

        protected PowerShell PowerShell { get; } = PowerShell.Create();

        public DomainClientCmdletTestBase()
        {
            this.PowerShell
                .AddCommand("Import-Module").AddParameter("Name", "./domain.client.psd1")
                .Invoke();

            if (this.PowerShell.HadErrors)
                throw new PSInvalidOperationException("test initialization fails");

            this.PowerShell.Commands.Clear();
        }
    }
}