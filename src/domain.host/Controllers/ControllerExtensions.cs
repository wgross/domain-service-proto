using domain.contract;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace domain.host.controllers
{
    public static class ControllerExtensions
    {
        public static async Task<IActionResult> InvokeServiceCommand<C, R>(this C controller, Func<Task<R>> serviceCommand)
            where C : ControllerBase
            where R : class
        {
            try
            {
                return controller.Ok(await serviceCommand());
            }
            catch (ArgumentNullException ex)
            {
                return controller.BadRequest(new DomainError
                {
                    ErrorType = nameof(ArgumentNullException),
                    ParamName = ex.ParamName,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return controller.BadRequest(new DomainError
                {
                    ErrorType = ex.GetType().Name,
                    Message = ex.Message
                });
            }
        }
    }
}