using GalleryMVC.Attributes;
using System.ComponentModel.DataAnnotations;

namespace GalleryMVC.Models
{
    public class UploadImageViewModel
    {
        [Required]
        [AllowedExtensions(
        new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" },
        new[] { "image/jpeg", "image/png", "image/gif", "image/webp" }
    )]
        public IFormFile File { get; init; }

        [Required]
        public string Title { get; init; }

        public string Description { get; init; }

        [Required]
        public int UserId { get; init; }
    }
}
