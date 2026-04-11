namespace GalleryMVC.Models
{
    public class ImageDetailsViewModel
    {
        public int ImgId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string UploadedBy { get; set; }
        public int UserId { get; set; }
        public DateTime UploadDate { get; set; }
    }
}
