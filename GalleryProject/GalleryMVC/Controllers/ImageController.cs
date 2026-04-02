using Gallery.BusinessLogic.Services;
using Gallery.DataAccess.Models;
using GalleryMVC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GalleryMVC.Controllers
{
    public class ImageController : Controller
    {
        private readonly ImageService _imageService;

        public ImageController(ImageService imageService)
        {
            _imageService = imageService;
        }

        // GET: ImageController
        public async Task<IActionResult> Index(string? search, string? filter)
        {
            var images = await _imageService.GetGalleryAsync(search, filter);
            return View(images);
        }
        public async Task<IActionResult> GetImageFile(int id)
        {
            var image = await _imageService.GetImageByIdAsync(id);
            if (image == null) return NotFound();

            // Повертаємо масив байтів як зображення
            return File(image.ImageData, image.ContentType);
        }

        [HttpGet]
        public IActionResult Upload() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(UploadImageViewModel request)
        {
            int? currentUserId = HttpContext.Session.GetInt32("UserId");

            if (currentUserId == null)
            {
                return RedirectToAction("Login", "User"); // Якщо сесія згасла — відправляємо логінитись
            }


            if (request.File == null || request.File.Length == 0)
            {
                ModelState.AddModelError("File", "Будь ласка, виберіть файл");
                return View(request);
            }

            using var memoryStream = new MemoryStream();
            await request.File.CopyToAsync(memoryStream);
            var entity = new Image
            {
                Title = request.Title,
                Description = request.Description,
                UserId = request.UserId,
                ImageData = memoryStream.ToArray(),
                ContentType = request.File.ContentType
            };
            await _imageService.UploadImageAsync(entity);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteImageAsync(int id)
        {
            await _imageService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
