using Microsoft.AspNetCore.Mvc.Filters;

namespace GalleryMVC.Filters
{
    public class ControllerLoggingFilter: IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            Console.WriteLine($"\n--- [CONTROLLER FILTER] Starting action: {context.ActionDescriptor.DisplayName}");
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            Console.WriteLine($"\n--- [CONTROLLER FILTER] Ending action: {context.ActionDescriptor.DisplayName}");
        }
    }
}
