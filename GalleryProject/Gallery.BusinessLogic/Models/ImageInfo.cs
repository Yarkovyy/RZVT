namespace Gallery.BusinessLogic.Models
{
    public class ImageInfo
    {
        public int ImgId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime UploadDate { get; set; }
        public string ImageUrl { get; set; }
        public int UserId { get; set; }
    }
}
