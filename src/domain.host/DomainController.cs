using domain.contract;
using domain.host;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace host
{
    [Route("domain")]
    public sealed class DomainController : ControllerBase
    {
        private readonly IDomainService domainService;

        public DomainController(IDomainService domainService)
        {
            this.domainService = domainService;
        }

        [HttpPost]
        public Task<IActionResult> DoSomething([FromBody] DoSomethingRequest request)
            => this.InvokeServiceCommand(() => this.domainService.DoSomething(request));
    }
}