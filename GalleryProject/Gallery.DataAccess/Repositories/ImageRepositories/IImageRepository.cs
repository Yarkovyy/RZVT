using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gallery.DataAccess.Models;

namespace Gallery.DataAccess.Repositories.ImageRepositories
{
    public interface IImageRepository
    {
        public IQueryable<Image> GetAll();
        public Task<Image?> GetByIdAsync(int id);
        public Task AddAsync(Image image);
        public Task UpdateAsync(Image image);
        public Task DeleteAsync(int id);
    }
}
