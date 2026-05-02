using GalleryMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.VisualStudio.Web.CodeGeneration.EntityFrameworkCore;
using System.Diagnostics;

namespace GalleryMVC.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly IModelMetadataProvider _modelMetadataProvider;
        public GlobalExceptionFilter(IModelMetadataProvider modelMetadataProvider)
        {
            _modelMetadataProvider = modelMetadataProvider;
        }
        public void OnException(ExceptionContext context)
        {
            Console.WriteLine($"\n--- [GLOBAL FILTER] Exception caught: {context.Exception.Message}");
            context.Result = new RedirectToActionResult("Error", "Home", new
            {
                message = "A critical server error occurred. Our developers are already looking into it."
            });
            context.ExceptionHandled = true;
        }
    }
}
