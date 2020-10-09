using domain.contract;
using Microsoft.AspNetCore.Mvc;
using System;
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

        [HttpPost, Route("do")]
        public Task<IActionResult> DoSomething([FromBody] DoSomethingRequest request)
            => this.InvokeServiceCommand(() => this.domainService.DoSomething(request));

        [HttpPost]
        public Task<IActionResult> CreateEntity([FromBody] CreateDomainEntityRequest request)
            => this.InvokeServiceCreateCommand(() => this.domainService.CreateEntity(request));

        [HttpGet, Route("{id:Guid}")]
        public Task<IActionResult> GetEntity([FromRoute] Guid id)
            => this.InvokeServiceCommandAtRequiredResource(() => this.domainService.GetEntity(id));

        [HttpGet]
        public Task<IActionResult> GetEntities()
            => this.InvokeServiceCommand(() => this.domainService.GetEntities());

        [HttpDelete, Route("{id:Guid}")]
        public Task<IActionResult> DeleteEntity([FromRoute] Guid id)
            => this.InvokeServiceCommand(() => this.domainService.DeleteEntity(id));
    }
}