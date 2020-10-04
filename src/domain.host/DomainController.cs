using domain.contract;
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
        public async Task<IActionResult> DoSomething([FromBody] DoSomethingRequest request) => this.Ok(await this.domainService.DoSomething(request));
    }
}