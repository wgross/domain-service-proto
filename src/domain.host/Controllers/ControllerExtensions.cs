using domain.contract;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace domain.host.controllers
{
    /// <summary>
    /// These extensions provide an opinonated implementation of controller endppoint behavior.
    /// They translate commoan domain behavior to the HTTP protocol.
    /// </summary>
    public static class ControllerExtensions
    {
        public static async Task<IActionResult> MapExceptions<C>(this C controller, Func<Task<IActionResult>> action)
            where C : ControllerBase
        {
            try
            {
                return await action();
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

        public static Task<IActionResult> InvokeServiceCommand<C, R>(this C controller, Func<Task<R>> serviceCommand)
            where C : ControllerBase
            where R : class
        {
            return controller.MapExceptions(async () => controller.Ok(await serviceCommand()));
        }

        public static Task<IActionResult> InvokeServiceCommandAtRequiredResource<C, R>(this C controller, Func<Task<R>> serviceCommand)
            where C : ControllerBase
            where R : class
        {
            return controller.MapExceptions(async () =>
            {
                var response = await serviceCommand();
                if (response is null)
                    return controller.NotFound();
                return controller.Ok(await serviceCommand());
            });
        }

        public static Task<IActionResult> InvokeServiceCommand<C>(this C controller, Func<Task> serviceCommand)
            where C : ControllerBase
        {
            return controller.MapExceptions(async () =>
            {
                await serviceCommand();
                return controller.NoContent();
            });
        }

        /// <summary>
        /// The common behavior for delete enetpints is to return <see cref="NoContentResult"/> on success and
        /// <see cref="NotFoundResult"/> if the entity doesn exists any more. Other error causes must be passed as exceptions.
        /// </summary>
        /// <typeparam name="C"></typeparam>
        /// <param name="controller"></param>
        /// <param name="deleteCommand"></param>
        /// <returns></returns>
        public static Task<IActionResult> InvokeDeleteCommand<C>(this C controller, Func<Task<bool>> deleteCommand)
            where C : ControllerBase
        {
            return controller.MapExceptions(async () =>
            {
                return await deleteCommand() switch
                {
                    true => controller.NoContent(),
                    false => controller.NotFound()
                };
            });
        }
    }
}