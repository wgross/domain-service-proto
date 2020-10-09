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
        public async Task<IActionResult> CreateEntity([FromBody] CreateDomainEntityRequest request)
        {
            try
            {
                var result = await this.domainService.CreateEntity(request);
                return this.CreatedAtAction(actionName: nameof(GetEntity), routeValues: new { id = result.Id }, result);
            }
            catch (ArgumentNullException ex)
            {
                return this.BadRequest(new DomainError
                {
                    ErrorType = nameof(ArgumentNullException),
                    ParamName = ex.ParamName,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return this.BadRequest(new DomainError
                {
                    ErrorType = ex.GetType().Name,
                    Message = ex.Message
                });
            }
        }

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