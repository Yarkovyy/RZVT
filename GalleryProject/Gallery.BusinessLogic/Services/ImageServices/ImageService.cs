using Gallery.BusinessLogic.Models;
using Gallery.DataAccess.Models;
using Gallery.DataAccess.Repositories.ImageRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gallery.BusinessLogic.Services.ImageServices
{
    public sealed class ImageService : IImageService
    {
        private const string ImageGetByIdRoute = "/api/images/getById/";
        private readonly IImageRepository _imageRepository;

        public ImageService(IImageRepository imageRepository)
        {
            _imageRepository = imageRepository ?? throw new ArgumentNullException(nameof(imageRepository));
        }

        public async Task<Image> GetImageByIdAsync(int id)
        {
            return await _imageRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<ImageInfo>> GetGalleryAsync(string? search, string? filter)
        {
            var images = _imageRepository.GetAll();
            if (!string.IsNullOrWhiteSpace(search))
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
                ImageUrl = $"{ImageGetByIdRoute}{i.ImgId}"
            }).ToListAsync();
        }

        public async Task<IEnumerable<ImageInfo>> GetImagesByUserIdAsync(int? id)
        {
            var images = _imageRepository.GetAll();
            images = images.Where(i => i.UserId == id);
            return await images.Select(i => new ImageInfo
            {
                ImgId = i.ImgId,
                Title = i.Title,
                Description = i.Description,
                UploadDate = i.UploadDate,
                ImageUrl = $"{ImageGetByIdRoute}{i.ImgId}"
            }).ToListAsync();
        }


        public async Task UploadImageAsync(Image image)
        {
            image.UploadDate = DateTime.Now;
            await _imageRepository.AddAsync(image);
        }
        public async Task UpdateImageInfoAsync(int id, string title, string description)
        {
            var image = await _imageRepository.GetByIdAsync(id);
            if (image == null) return;
            image.Title = title;
            image.Description = description;
            await _imageRepository.UpdateAsync(image);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var image = await _imageRepository.GetByIdAsync(id);
            if (image == null) return false;
       
            await _imageRepository.DeleteAsync(id);
            return true;
        }
    }
}
