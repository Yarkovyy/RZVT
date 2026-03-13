using Gallery.BusinessLogic.Models;
using Gallery.BusinessLogic.Services;
using Gallery.DataAccess.Models;
using Gallery.Models;
using Microsoft.AspNetCore.Mvc;


namespace Gallery.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly ImageService _imageService;

        public ImageController(ImageService imageService)
        {
            _imageService = imageService;
        }

        // ЗАВАНТАЖЕННЯ (Використовуємо FromForm для передачі файлу)  
        [HttpPost("upload")]
        public async Task<ActionResult> Upload([FromForm] UploadImageRequest request)
        {
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
            // Повертаємо список без самих байтів (ImageData = null), щоб не перевантажувати мережу
            //var result = images.Select(i => new ImageInfo
            //{
            //    ImgId = i.ImgId,
            //    Title = i.Title,
            //    Description = i.Description,
            //    UploadDate = i.UploadDate,
            //    ImageUrl = $"/Image/getById/{i.ImgId}"
            //});
            return Ok(images);
        }
        [HttpDelete("delete/{id}", Name = "DeleteImageById")]
        public async Task<IActionResult> DeleteImageAsync(int id)
        {
            await _imageService.DeleteAsync(id);
            return Ok("Зображення видалено");
        }
    }
}
