using System.ComponentModel.DataAnnotations;

namespace GalleryMVC.Models
{
    public class EditImageViewModel
    {
        public int ImgId { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(100)]
        public string Title { get; set; }
        [StringLength(500)]
        public string? Description { get; set; }
    }
}
