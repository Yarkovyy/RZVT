using Gallery.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace Gallery.DataAccess
{
    public class GalleryContext: DbContext
    {
        public GalleryContext(DbContextOptions<GalleryContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Image> Images { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Створюємо тестового юзера
            modelBuilder.Entity<User>().HasData(
                new User { UserId = 1, Email = "test@example.com", Password = "password_here" }
            );
        }

    }
}
