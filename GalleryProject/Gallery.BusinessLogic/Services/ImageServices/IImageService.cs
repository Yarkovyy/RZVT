using Gallery.BusinessLogic.Models;
using Gallery.DataAccess.Models;

namespace Gallery.BusinessLogic.Services.ImageServices
{
    public interface IImageService
    {
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<ImageInfo>> GetGalleryAsync(string? search, string? filter);
        Task<Image> GetImageByIdAsync(int id);
        Task UploadImageAsync(Image image);
    }
}