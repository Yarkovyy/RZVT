using Gallery.BusinessLogic.Models;
using Gallery.BusinessLogic.Services.ImageServices;
using Gallery.BusinessLogic.Services.UserService;
using Gallery.DataAccess.Models;
using GalleryMVC.Filters;
using GalleryMVC.Models;
using GalleryMVC.SignalR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.SignalR;

namespace GalleryMVC.Controllers
{
    [ServiceFilter(typeof(ControllerLoggingFilter))]
    public class ImageController : Controller
    {
        private readonly IImageService _imageService;
        private readonly IUserService _userService;
        private readonly IHubContext<GalleryHub> _hubContext;

        public ImageController(IImageService imageService, IUserService userService, IHubContext<GalleryHub> hubContext)
        {
            _imageService = imageService;
            _userService = userService;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> Index(string? search, string? filter)
        {
            //throw new Exception("Exeption for test GlobalExceptionFilter");
            var images = await _imageService.GetGalleryAsync(search, filter);
            return View(images);
        }
        public async Task<IActionResult> GetUserGallery()
        {
            int? currentUserId = HttpContext.Session.GetInt32("UserId");

            if (currentUserId == null)
            {
                return RedirectToAction("Login", "User");
            }

            var images = await _imageService.GetImagesByUserIdAsync(currentUserId);
            return View(images);
        }

        [TypeFilter(typeof(ValidateEntityIdFilter), Arguments = new object[] { "Image" })]
        public async Task<IActionResult> GetImageFile(int id)
        {
            var image = await _imageService.GetImageByIdAsync(id);
            if (image == null)
            {
                return RedirectToError("Image file not found.");
            }

            return File(image.ImageData, image.ContentType);
        }

        [TypeFilter(typeof(ValidateEntityIdFilter), Arguments = new object[] { "Image" })]
        public async Task<IActionResult> Details(int id)
        {
            var image = await _imageService.GetImageByIdAsync(id);
            if (image == null)
            {
                return RedirectToError("We couldn't find the details for this image.");
            }

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
                return RedirectToAction("Login", "User"); 
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



            var imageInfo = new ImageInfo
            {
                ImgId = entity.ImgId,
                Title = entity.Title,
                Description = entity.Description,
                UploadDate = DateTime.Now,
                UserId = entity.UserId,
                ImageUrl = Url.Action("GetImageFile", "Image", new { id = entity.ImgId })
            };

            await _hubContext.Clients.All.SendAsync("ReceiveNewImage", imageInfo);

            return RedirectToAction(nameof(Index));
        }



        public async Task<IActionResult> Edit(int id)
        {
            var image = await _imageService.GetImageByIdAsync(id);
            if (image == null)
            {
                return RedirectToError($"Image with ID {id} not found.");
            }
            int? currentUserId = HttpContext.Session.GetInt32("UserId");
            if (currentUserId == null || image.UserId != currentUserId)
            {
                return RedirectToError("Access denied. You can only edit your own images.");
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
            if (image == null) return RedirectToError("Image not found for editing.");
            int? currentUserId = HttpContext.Session.GetInt32("UserId");
            if (currentUserId == null || image.UserId != currentUserId)
            {
                return RedirectToError("You do not have permission to modify this image.");
            }
            await _imageService.UpdateImageInfoAsync(model.ImgId, model.Title, model.Description);

            return RedirectToAction("Details", new { id = model.ImgId });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        [TypeFilter(typeof(ValidateEntityIdFilter), Arguments = new object[] { "Image" })]
        public async Task<IActionResult> DeleteImageAsync(int id)
        {

            var image = await _imageService.GetImageByIdAsync(id);

            if (image == null)
            {
                return RedirectToError($"Cannot delete image with ID {id} because it doesn't exist.");
            }

            int? currentUserId = HttpContext.Session.GetInt32("UserId");

            if (currentUserId == null || image.UserId != currentUserId)
            {
                return RedirectToError("Permission denied. Deletion is restricted to the owner.");
            }

            bool isDeleted = await _imageService.DeleteAsync(id);

            if (isDeleted)
            {
                return RedirectToAction(nameof(Index));
            }

            return RedirectToError("A server error occurred while trying to delete the image.");
        }
        private RedirectToActionResult RedirectToError(string message)
        {
            return RedirectToAction("Error", "Home", new { message = message });
        }
    }
}
