using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gallery.DataAccess.Models;
using Gallery.DataAccess.Repositories.UserRepositories;

namespace Gallery.BusinessLogic.Services
{
    public class UserService
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
                throw new ArgumentException("Невірний формат Email.");
            }

            var existingUser = await _userRepository.GetByEmailAsync(email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Користувач з таким Email вже існує.");
            }

            var user = new User { Email = email, Password = password }; 
            await _userRepository.AddAsync(user);
        }

        public async Task<User?> LoginAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user != null && user.Password == password) return user;
            throw new UnauthorizedAccessException("Невірний Email або пароль.");
        }
    }
}
