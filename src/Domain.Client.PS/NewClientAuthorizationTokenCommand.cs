using Domain.Host.Controllers;
using System;
using System.Management.Automation;

namespace Domain.Client.PS
{
    [Cmdlet(VerbsCommon.New, "ClientAuthorizationToken")]
    public sealed class NewClientAuthorizationTokenCommand : PSCmdlet
    {
        [Parameter(Mandatory = true)]
        public Uri Authority { get; set; }

        [Parameter(Mandatory = true)]
        public string ClientId { get; set; }

        [Parameter(Mandatory = true)]
        public string ClientSecret { get; set; }

        [Parameter(Mandatory = true)]
        public string[] Scopes { get; set; }

        protected override void ProcessRecord()
        {
            AsyncHelper.RunSync(async () =>
            {
                DomainClientSession.Setup(opts =>
                {
                    opts.Authority = this.Authority;
                });
                await DomainClientSession.CurrentTokenProvider.FetchToken(this.ClientId, this.ClientSecret, this.Scopes);
            });
        }
    }
}