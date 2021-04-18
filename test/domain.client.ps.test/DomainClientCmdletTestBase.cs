using Domain.Host.TestServer;
using Moq;
using System.Management.Automation;
using Xunit;

namespace Domain.Client.PS.Test
{
    [Collection("UsesPowershell")]
    public abstract class DomainClientCmdletTestBase
    {
        protected MockRepository Mocks { get; } = new MockRepository(MockBehavior.Strict);

        protected PowerShell PowerShell { get; } = PowerShell.Create();

        protected AuthorityTestServerFactory Authority { get; private set; }

        protected DomainHostTestServer Host { get; }

        public DomainClientCmdletTestBase()
        {
            this.ArrangeAuthority();

            this.Host = new DomainHostTestServer(this.Authority.Server);
            DomainClientSession.DomainClientHandler = this.Host.Server.CreateHandler();

            this.PowerShell
                .AddCommand("Import-Module").AddParameter("Name", "./domain.client.psd1")
                .Invoke();

            this.ArrangeClientSession();

            if (this.PowerShell.HadErrors)
                throw new PSInvalidOperationException("test initialization fails");

            this.PowerShell.Commands.Clear();
        }

        #region Arrange Test Environment

        protected void ArrangeClientSession() => DomainClientSession.Setup(opts =>
        {
            opts.Authority = this.Authority.Server.BaseAddress;
        });

        protected void ArrangeToken()
        {
            this.PowerShell
                .AddCommand("New-ClientAuthorizationToken")
                    .AddParameter("Authority", this.Authority.Server.BaseAddress)
                    .AddParameter("ClientId", "client")
                    .AddParameter("ClientSecret", "password123$")
                    .AddParameter("Scopes", new[] { "DomainService" })
                .Invoke();

            this.PowerShell.Commands.Clear();
        }

        public void ArrangeAuthority()
        {
            this.Authority = new AuthorityTestServerFactory();
            this.Authority.AddClientCredentials("client", "password123$", "DomainService");
            DomainClientSession.AuthorityClientOverride = this.Authority.Server.CreateClient();
        }

        #endregion Arrange Test Environment
    }
}