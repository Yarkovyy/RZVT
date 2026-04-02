using Gallery.BusinessLogic.Services;
using GalleryMVC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GalleryMVC.Controllers
{
    public class UserController : Controller
    {
        private readonly UserService _userService;
        public UserController(UserService userService)
        {
            _userService = userService;
        }
        // GET: UserController
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterOrLoginUserViewModel request)
        {
            if (!ModelState.IsValid)
                return View(request);
            try
            {
                await _userService.RegisterAsync(request.Email, request.Password);
                return RedirectToAction(nameof(Login));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Помилка реєстрації: " + ex.Message);
                return View(request);
            }
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(RegisterOrLoginUserViewModel request)
        {
            if (!ModelState.IsValid)
                return View(request);

            try
            {
                var user = await _userService.LoginAsync(request.Email, request.Password);

                // Записуємо ID користувача в сесію
                HttpContext.Session.SetInt32("UserId", user.UserId);
                HttpContext.Session.SetString("UserEmail", user.Email);

                return RedirectToAction("Index", "Image");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Невірний логін або пароль");
                return View(request);
            }
        }
        
        public IActionResult Logout()
        {
            // Повністю видаляємо всі дані з сесії
            HttpContext.Session.Clear();

            // Повертаємо користувача в галерею
            return RedirectToAction("Index", "Image");
        }

    }
}
