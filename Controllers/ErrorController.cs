using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace DemoApi.Controllers
{
    [ApiController]
    public class ErrorController : ControllerBase
    {
        [HttpGet("/error")]
        public IActionResult Error()
        {
            IExceptionHandlerFeature context = HttpContext.Features.Get<IExceptionHandlerFeature>();

            if (context is not null)
            {
                string stackTrace = context.Error.StackTrace;
                string errorMessage = context.Error.Message;

                // Log Problem 
            }

            return Problem();
        }
    }
}
