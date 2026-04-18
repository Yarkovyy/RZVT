using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gallery.DataAccess.Models;

namespace Gallery.DataAccess.Repositories.ImageRepositories
{
    public class ImageRepository : IImageRepository
    {
        private readonly GalleryContext _context;
        public ImageRepository(GalleryContext context) => _context = context;

        public IQueryable<Image> GetAll() =>
            _context.Images.AsNoTracking(); 

        public async Task<Image?> GetByIdAsync(int id) =>
            await _context.Images.FindAsync(id);

        public async Task AddAsync(Image image)
        {
            await _context.Images.AddAsync(image);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            int rowsAffected = await _context.Images
            .Where(i => i.ImgId == id)
            .ExecuteDeleteAsync();
        }
    }
}
