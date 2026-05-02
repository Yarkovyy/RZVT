using Gallery.BusinessLogic.Services.ImageServices;
using Gallery.BusinessLogic.Services.UserService;
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
        private readonly IUserService _userService;

        //private static readonly string[] AllowedContentTypes = { "image/jpeg", "image/png", "image/gif", "image/webp" };
        //private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

        public ImageController(IImageService imageService, IUserService userService)
        {
            _imageService = imageService;
            _userService = userService;
        }

        public async Task<IActionResult> Index(string? search, string? filter)
        {
            var images = await _imageService.GetGalleryAsync(search, filter);
            return View(images);
        }
        public async Task<IActionResult> GetUserGallery()
        {
            int? currentUserId = HttpContext.Session.GetInt32("UserId");

            if (currentUserId == null)
            {
                return RedirectToAction("Login", "User"); // Якщо сесія згасла — відправляємо логінитись
            }

            var images = await _imageService.GetImagesByUserIdAsync(currentUserId);
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
        public async Task<IActionResult> Details(int id)
        {

            if (id <= 0)
            {
                return NotFound();
            }
            var image = await _imageService.GetImageByIdAsync(id);
            if (image == null) return NotFound();

            var img = new ImageDetailsViewModel
            {
                ImgId = image.ImgId,
                Title = image.Title,
                Description = image.Description,
                UploadedBy = await _userService.GetEmailByIdAsync(image.UserId),
                UserId = image.UserId,
                UploadDate = image.UploadDate
            };

            return View(img);
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

            //if (request.File == null || request.File.Length == 0)
            //{
            //    ModelState.AddModelError("File", "Please select a file");
            //    return View(request);
            //}
            //var extension = Path.GetExtension(request.File.FileName).ToLowerInvariant();
            //if (!AllowedContentTypes.Contains(request.File.ContentType) || !AllowedExtensions.Contains(extension))
            //{
            //    ModelState.AddModelError("File", "Invalid file format. Allowed formats: JPEG, PNG, GIF, WEBP.");
            //    return View(request);
            //}

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



        public async Task<IActionResult> Edit(int id)
        {            
            var image = await _imageService.GetImageByIdAsync(id);
            if (image == null)
            {
                return NotFound($"Image with ID {id} not found.");
            }
            int? currentUserId = HttpContext.Session.GetInt32("UserId");
            if (currentUserId == null || image.UserId != currentUserId)
            {
                return Forbid();
            }

            var model = new EditImageViewModel
            {
                ImgId = image.ImgId,
                Title = image.Title,
                Description = image.Description
            };
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditImageViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var image = await _imageService.GetImageByIdAsync(model.ImgId);
            if (image == null) return NotFound();            
            int? currentUserId = HttpContext.Session.GetInt32("UserId");
            if (currentUserId == null || image.UserId != currentUserId)
            {
                return Forbid();
            }
            await _imageService.UpdateImageInfoAsync(model.ImgId, model.Title, model.Description);

            return RedirectToAction("Details", new { id = model.ImgId });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteImageAsync(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Image ID.");
            }

            var image = await _imageService.GetImageByIdAsync(id);

            if (image == null)
            {
                return NotFound($"Image with ID {id} not found.");
            }

            int? currentUserId = HttpContext.Session.GetInt32("UserId");

            if (currentUserId == null || image.UserId != currentUserId)
            {
                return Forbid();
            }

            bool isDeleted = await _imageService.DeleteAsync(id);

            if (isDeleted)
            {
                return RedirectToAction(nameof(Index));
            }

            return StatusCode(500, "Internal server error during deletion.");
        }
    }
}
