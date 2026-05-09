using Gallery.DataAccess.Models;
using Gallery.DataAccess.Repositories.UserRepositories;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gallery.BusinessLogic.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        public UserService(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task RegisterAsync(string username, string email, string password)
        {
            var user = new User { UserName = username, Email = email };
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Registration failed: {errors}");
            }
        }
        public async Task LoginAsync(string usernameOrEmail, string password, bool rememberMe)
        {
            var user = await _userManager.FindByNameAsync(usernameOrEmail) ?? await _userManager.FindByEmailAsync(usernameOrEmail);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid login attempt.");
            }
            
            var result = await _signInManager.PasswordSignInAsync(user, password, rememberMe, lockoutOnFailure: false);

            if(!result.Succeeded)
            {
                throw new UnauthorizedAccessException("Invalid login attempt.");
            }
        }

        public async Task<string> GetEmailByIdAsync(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            return user?.Email ?? "Unknown User";
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
