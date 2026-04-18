using System.ComponentModel.DataAnnotations;

namespace Gallery.Models
{
    public class UploadImageRequest
    {
        [Required]
        public IFormFile File { get; init; }

        [Required]
        public string Title { get; init; }

        public string Description { get; init; }

        [Required]
        public int UserId { get; init; }
    }
}
