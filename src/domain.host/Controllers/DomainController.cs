using domain.contract;
using Microsoft.AspNetCore.Http;
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
        [Consumes("application/json")]
        [ProducesResponseType(typeof(DoSomethingResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DomainError), StatusCodes.Status400BadRequest)]
        public Task<IActionResult> DoSomething([FromBody] DoSomethingRequest request)
            => this.InvokeServiceCommand(() => this.domainService.DoSomething(request));

        [HttpPost]
        [ProducesResponseType(typeof(DomainEntityResult), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(DomainError), StatusCodes.Status400BadRequest)]
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
        [ProducesResponseType(typeof(DomainEntityResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Task<IActionResult> GetEntity([FromRoute] Guid id)
            => this.InvokeServiceCommandAtRequiredResource(() => this.domainService.GetEntity(id));

        [HttpGet]
        [ProducesResponseType(typeof(DomainEntityCollectionResult), StatusCodes.Status200OK)]
        public Task<IActionResult> GetEntities()
            => this.InvokeServiceCommand(() => this.domainService.GetEntities());

        [HttpDelete, Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Task<IActionResult> DeleteEntity([FromRoute] Guid id)
            => this.InvokeDeleteCommand(() => this.domainService.DeleteEntity(id));
    }
}