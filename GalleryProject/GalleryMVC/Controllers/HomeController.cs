using GalleryMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GalleryMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string? message)
        {
            var Message = message
                       ?? ViewData["CustomErrorMessage"]?.ToString()
                       ?? "An unexpected error occurred.";

            return View(new ErrorViewModel
            {
                RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                Message = Message ?? "An unexpected error occurred."
            });
        }
    }
}
