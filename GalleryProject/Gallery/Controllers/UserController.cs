using Gallery.BusinessLogic.Services.UserService;
using Gallery.Models;
using Microsoft.AspNetCore.Mvc;

namespace Gallery.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController: ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                await _userService.RegisterAsync(request.Username, request.Email, request.Password);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                await _userService.LoginAsync(request.UsernameOrEmail, request.Password, request.RememberMe);
                return Ok(new { request.UsernameOrEmail, request.Password, request.RememberMe });
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
        }
    }
}
