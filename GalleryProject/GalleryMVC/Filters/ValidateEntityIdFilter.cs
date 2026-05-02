using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GalleryMVC.Filters
{
    public class ValidateEntityIdFilter : IActionFilter
    {
        private readonly string _entityName;
        public ValidateEntityIdFilter(string entityName)
        {
            _entityName = entityName;
        }
        public void OnActionExecuting(ActionExecutingContext context)
        {
            Console.WriteLine($"--- [ACTION FILTER] Executing validation for: {context.ActionDescriptor.DisplayName}");
            if (context.ActionArguments.TryGetValue("id", out var idObj) && idObj is int id)
            {
                if (id <= 0)
                {
                    Console.WriteLine($"--- [ACTION FILTER] Validation failed for {_entityName} with ID: {id}");

                    context.Result = new RedirectToActionResult("Error", "Home", new
                    {
                        message = $"Invalid {_entityName} ID. The identifier must be a positive integer."
                    });
                }
            }
            else
            {
                Console.WriteLine($"--- [ACTION FILTER] Error: 'id' is missing or has a wrong format.");
                context.Result = context.Result = new RedirectToActionResult("Error", "Home", new
                {
                    message = $"{_entityName} ID is required and must be a valid number."
                });
            }
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
