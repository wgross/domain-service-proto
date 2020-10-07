using domain.contract;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace domain.host.controllers
{
    [Route("domain")]
    public sealed class DomainController : ControllerBase
    {
        private readonly IDomainService domainService;

        public DomainController(IDomainService domainService)
        {
            this.domainService = domainService;
        }

        [HttpPost("do")]
        public Task<IActionResult> DoSomething([FromBody] DoSomethingRequest request)
            => this.InvokeServiceCommand(() => this.domainService.DoSomething(request));
    }
}