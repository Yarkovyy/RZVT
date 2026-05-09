using Gallery.DataAccess.Models;

namespace Gallery.BusinessLogic.Services.UserService
{
    public interface IUserService
    {
        Task RegisterAsync(string username, string email, string password);
        Task LoginAsync(string usernameOrEmail, string password, bool rememberMe);
        Task LogoutAsync();
        Task<string> GetEmailByIdAsync(int id);

    }
}