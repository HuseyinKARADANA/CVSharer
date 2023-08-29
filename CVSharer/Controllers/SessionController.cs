using BusinessLayer.Abstract;
using EntityLayer.Concrete;
using EntityLayer.DTOs;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;


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
                IsActive = true
            });

            _toast.AddSuccessToastMessage("Register Successful.", new ToastrOptions { Title = "Successful" });

            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login()
        {
            ClaimsPrincipal claimUser = HttpContext.User;

            if (claimUser.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            var isCredentialsValid = false;
            User validUser = new();

            List<User> userList = _userService.GetListAll();

            foreach (var user in userList)
            {
                try
                {
                    if (BCrypt.Net.BCrypt.Verify(loginDTO.Password, user.Password) && user.Email == loginDTO.Email)
                    {
                        isCredentialsValid = true;
                        validUser = user;
                        break;
                    }
                }
                catch (Exception ex)
                {
                    _toast.AddErrorToastMessage("An unexpected error is encountered. Please try again later.", new ToastrOptions { Title = "Error" });
                    return View();
                }
            }

            if (!isCredentialsValid)
            {
                _toast.AddErrorToastMessage("Email or password are incorrect, please try again.", new ToastrOptions { Title = "Error" });
                return View();
            }

            if (!validUser.IsActive)
            {
                _toast.AddErrorToastMessage("This user is not active!", new ToastrOptions { Title = "Error" });
                return View();
            }

            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, loginDTO.Email),
                new Claim(ClaimTypes.Name, validUser.Name +" "+ validUser.Surname),
                new Claim(ClaimTypes.Role, "User")
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true,
                IsPersistent = loginDTO.KeepLoggedIn,
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), properties);

            HttpContext.Response.Cookies.Append("UserId", validUser.UserId.ToString());

            _toast.AddSuccessToastMessage("Login Successfully.", new ToastrOptions { Title = "Successful" });

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> LogOut()
        {
            HttpContext.Response.Cookies.Delete("UserId");
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
