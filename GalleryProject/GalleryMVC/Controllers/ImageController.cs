using Gallery.BusinessLogic.Models;
using Gallery.BusinessLogic.Services.ImageServices;
using Gallery.BusinessLogic.Services.UserService;
using Gallery.DataAccess.Models;
using GalleryMVC.Filters;
using GalleryMVC.Models;
using GalleryMVC.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.SignalR;

namespace GalleryMVC.Controllers
{
    [ServiceFilter(typeof(ControllerLoggingFilter))]
    public class ImageController : Controller
    {
        private readonly IImageService _imageService;
        private readonly IHubContext<GalleryHub> _hubContext;
        private readonly UserManager<User> _userManager;
        public ImageController(IImageService imageService, IHubContext<GalleryHub> hubContext, UserManager<User> userManager)
        {
            _imageService = imageService;
             _userManager = userManager;
           _hubContext = hubContext;
        }

        public async Task<IActionResult> Index(string? search, string? filter)
        {
            var images = await _imageService.GetGalleryAsync(search, filter);
            return View(images);
        }

        [Authorize]
        public async Task<IActionResult> GetUserGallery()
        {
            var userIdString = _userManager.GetUserId(User);
            if (userIdString == null) return RedirectToLogin();

            int currentUserId = int.Parse(userIdString);
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
            var owner = await _userManager.FindByIdAsync(image.UserId.ToString());

            var img = new ImageDetailsViewModel
            {
                ImgId = image.ImgId,
                Title = image.Title,
                Description = image.Description,
                UploadedBy = owner?.UserName ?? "Unknown",
                UserId = image.UserId,
                UploadDate = image.UploadDate
            };

            return View(img);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Upload() => View();

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(UploadImageViewModel request)
        {
            if (!ModelState.IsValid)
                return View(request);

            var userIdString = _userManager.GetUserId(User);
            if (userIdString == null)
            {
                return RedirectToLogin();
            }

            using var memoryStream = new MemoryStream();
            await request.File.CopyToAsync(memoryStream);
            var entity = new Image
            {
                Title = request.Title,
                Description = request.Description,
                UserId = int.Parse(userIdString),
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

        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var image = await _imageService.GetImageByIdAsync(id);
            if (image == null)
            {
                return RedirectToError($"Image with ID {id} not found.");
            }
            var userIdString = _userManager.GetUserId(User);
            if (userIdString == null || image.UserId != int.Parse(userIdString))
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

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditImageViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var image = await _imageService.GetImageByIdAsync(model.ImgId);
            if (image == null) return RedirectToError("Image not found for editing.");
            var userIdString = _userManager.GetUserId(User);
            if (userIdString == null || image.UserId != int.Parse(userIdString))
            {
                return RedirectToError("Permission denied.");
            }
            await _imageService.UpdateImageInfoAsync(model.ImgId, model.Title, model.Description);

            return RedirectToAction("Details", new { id = model.ImgId });
        }

        [Authorize]
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

            var userIdString = _userManager.GetUserId(User);
            if (userIdString == null || image.UserId != int.Parse(userIdString))
            {
                return RedirectToError("Permission denied.");
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
        private RedirectToActionResult RedirectToLogin()
        {
            return RedirectToAction("Login", "Account", new { area = "Identity" });
        }
    }
}
