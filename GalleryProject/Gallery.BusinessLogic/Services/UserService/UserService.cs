using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gallery.DataAccess.Models;
using Gallery.DataAccess.Repositories.UserRepositories;

namespace Gallery.BusinessLogic.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task RegisterAsync(string email, string password)
        {

            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
            {
                throw new ArgumentException("Invalid Email format.");
            }

            var existingUser = await _userRepository.GetByEmailAsync(email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("User with this Email already exists.");
            }

            var user = new User { Email = email, Password = password };
            await _userRepository.AddAsync(user);
        }

        public async Task<User?> LoginAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user != null && user.Password == password) return user;
            throw new UnauthorizedAccessException("Invalid Email or password.");
        }

        public async Task<string> GetEmailByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user?.Email ?? "Unknown User";
        }

        //public async Task<User?> GetUserByIdAsync(int id)
        //{
        //    return await _userRepository.GetByIdAsync(id);
        //}
    }
}
