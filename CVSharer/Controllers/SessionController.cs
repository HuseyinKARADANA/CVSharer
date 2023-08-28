using BusinessLayer.Abstract;
using EntityLayer.Concrete;
using EntityLayer.DTOs;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace CVSharer.Controllers
{
    public class SessionController : Controller
    {
        private readonly IUserService _userService;
        private readonly IToastNotification _toast;


        public SessionController(IUserService userService, IToastNotification toast)
        {
            _userService = userService;
            _toast = toast;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterDTO registerDTO)
        {
            List<User> users = _userService.GetListAll();

            if(users.Any(u => u.Email == registerDTO.Email))
            {
                _toast.AddErrorToastMessage("There is a user with this email. Please try another email.", new ToastrOptions { Title = "Error" });
                return View();
            }

            if(!registerDTO.Password.Equals(registerDTO.PasswordAgain))
            {
                _toast.AddErrorToastMessage("Passwords do not match!", new ToastrOptions { Title = "Error" });
                return View();
            }

            if(registerDTO.Password.Length < 8)
            {
                _toast.AddErrorToastMessage("Your password can be minimum 8 characters!", new ToastrOptions { Title = "Error" });
                return View();
            }

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDTO.Password);

            _userService.Insert(new User()
            {
                Name = registerDTO.Name,
                Surname = registerDTO.Surname,
                Email = registerDTO.Email,
                Password = hashedPassword,
            });

            _toast.AddSuccessToastMessage("Register Successful.", new ToastrOptions { Title = "Successful" });

            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(LoginDTO loginDTO)
        {
            return RedirectToAction("Login");
        }
    }
}
