using Gallery.DataAccess.Models;

namespace Gallery.BusinessLogic.Services.UserService
{
    public interface IUserService
    {
        Task<User?> LoginAsync(string email, string password);
        Task RegisterAsync(string email, string password);
        Task<string> GetEmailByIdAsync(int id);
    }
}