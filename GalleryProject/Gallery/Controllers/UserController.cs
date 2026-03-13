using Gallery.BusinessLogic.Services;
using Gallery.Models;
using Microsoft.AspNetCore.Mvc;

namespace Gallery.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController: ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterOrLoginUserRequest request)
        {
            try
            {
                await _userService.RegisterAsync(request.Email, request.Password);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] RegisterOrLoginUserRequest request)
        {
            try
            {
                var user = await _userService.LoginAsync(request.Email, request.Password);
                return Ok(new { user.UserId, user.Email });
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
        }
    }
}
