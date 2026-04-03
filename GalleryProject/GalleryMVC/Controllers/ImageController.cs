using Gallery.BusinessLogic.Services.ImageServices;
using Gallery.DataAccess.Models;
using GalleryMVC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GalleryMVC.Controllers
{
    public class ImageController : Controller
    {
        private readonly IImageService _imageService;

        private static readonly string[] AllowedContentTypes = { "image/jpeg", "image/png", "image/gif", "image/webp" };
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

        public ImageController(IImageService imageService)
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

            if (id <= 0)
            {
                return NotFound();
            }
            var image = await _imageService.GetImageByIdAsync(id);
            if (image == null) return NotFound();
            
            return File(image.ImageData, image.ContentType);
        }

        [HttpGet]
        public IActionResult Upload() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(UploadImageViewModel request)
        {
            if (!ModelState.IsValid)
                return View(request);


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
            var extension = Path.GetExtension(request.File.FileName).ToLowerInvariant();
            if (!AllowedContentTypes.Contains(request.File.ContentType) || !AllowedExtensions.Contains(extension))
            {
                ModelState.AddModelError("File", "Невірний формат файлу. Дозволені формати: JPEG, PNG, GIF, WEBP.");
                return View(request);
            }

            using var memoryStream = new MemoryStream();
            await request.File.CopyToAsync(memoryStream);
            var entity = new Image
            {
                Title = request.Title,
                Description = request.Description,
                UserId = (int)currentUserId,
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
            if (id <= 0)
            {
                return BadRequest("Невірний ID зображення.");
            }

            if (await _imageService.DeleteAsync(id))
                return RedirectToAction(nameof(Index));
            return NotFound($"Зображення з ID {id} не знайдено.");
        }
    }
}
