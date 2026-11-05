using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Simple_API_Assessment.Filters
{
    //Returns 422 Unprocessable Entity with the validation errors when the model state is invalid.
    //It runs before the [ApiController] automatic 400 response (order -2000), so it only changes
    //the status code of the controllers or actions it is applied to.
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class UnprocessableEntityOnInvalidModelAttribute : Attribute, IActionFilter, IOrderedFilter
    {
        public int Order => -3000;

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var problem = new ValidationProblemDetails(context.ModelState)
                {
                    Status = StatusCodes.Status422UnprocessableEntity
                };
                context.Result = new UnprocessableEntityObjectResult(problem);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
