using Gallery.BusinessLogic.Models;
using Gallery.DataAccess.Models;
using Gallery.DataAccess.Repositories.ImageRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Gallery.BusinessLogic.Services
{
    public class ImageService
    {
        private readonly IImageRepository _imageRepository;

        public ImageService(IImageRepository imageRepository)
        {
            _imageRepository = imageRepository;
        }

        public async Task<Image> GetImageByIdAsync(int id)
        {
            return await _imageRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<ImageInfo>> GetGalleryAsync(string? search, string? filter)
        {
            var images = _imageRepository.GetAll();
            if (!string.IsNullOrEmpty(search))
            {
                images = images.Where(i =>
                    i.Title.Contains(search) || i.Description.Contains(search));
            }

            if (filter == "newest")
            {
                images = images.OrderByDescending(i => i.UploadDate);
            }
            else if (filter == "oldest")
            {
                images = images.OrderBy(i => i.UploadDate);
            }
            return await images.Select(i => new ImageInfo
            {
                ImgId = i.ImgId,
                Title = i.Title,
                Description = i.Description,
                UploadDate = i.UploadDate,
                ImageUrl = $"/Image/getById/{i.ImgId}"
            }).ToListAsync();
        }

        public async Task UploadImageAsync(Image image)
        {
            image.UploadDate = DateTime.Now;
            await _imageRepository.AddAsync(image);
        }
        public async Task DeleteAsync(int id)
        {
            await _imageRepository.DeleteAsync(id);
        }
    }
}
