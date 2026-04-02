using Gallery.BusinessLogic.Models;
using Gallery.BusinessLogic.Services.ImageServices;
using Gallery.DataAccess.Models;
using Gallery.Models;
using Microsoft.AspNetCore.Mvc;


namespace Gallery.Controllers
{
    [ApiController]
    [Route("api/images")]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;
        private static readonly string[] AllowedContentTypes = { "image/jpeg", "image/png", "image/gif", "image/webp" };
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

        public ImageController(IImageService imageService)
        {
            _imageService = imageService;
        }

        // ЗАВАНТАЖЕННЯ (Використовуємо FromForm для передачі файлу)  
        [HttpPost("upload")]
        public async Task<ActionResult> Upload([FromForm] UploadImageRequest request)
        {
            if (request.File == null || request.File.Length == 0)
            {
                return BadRequest("Файл не вибрано або він пошкоджений (порожній).");
            }

            var extension = Path.GetExtension(request.File.FileName).ToLowerInvariant();
            if (!AllowedContentTypes.Contains(request.File.ContentType) || !AllowedExtensions.Contains(extension))
            {
                return BadRequest("Невірний формат файлу. Дозволені формати: JPEG, PNG, GIF, WEBP.");
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
            return Ok("Зображення збережено в БД");
        }

        // ПЕРЕГЛЯД (Повертаємо файл, щоб браузер його відобразив)
        [HttpGet("getById/{id}", Name = "GetImageById")]
        public async Task<IActionResult> GetImageById([FromRoute] int id)
        {
            if(id <= 0)
            {
                return BadRequest("Невірний ID зображення.");
            }

            var image = await _imageService.GetImageByIdAsync(id);
            if (image == null) return NotFound();

            // Повертаємо масив байтів як зображення
            return File(image.ImageData, image.ContentType);
        }

        // ПОШУК ТА ФІЛЬТРАЦІЯ
        [HttpGet("get", Name = "GetGallery")]
        public async Task<IActionResult> GetGallery([FromQuery] string? search, [FromQuery] string? filter)
        {           
            var images = await _imageService.GetGalleryAsync(search, filter);
            return Ok(images);
        }
        [HttpDelete("delete/{id}", Name = "DeleteImageById")]
        public async Task<IActionResult> DeleteImageAsync(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Невірний ID зображення.");
            }
            if(await _imageService.DeleteAsync(id))
                return Ok("Зображення видалено");
            return NotFound($"Зображення з ID {id} не знайдено.");
        }
    }
}
