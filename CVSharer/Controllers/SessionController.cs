using Microsoft.AspNetCore.Mvc;

namespace CVSharer.Controllers
{
    public class SessionController : Controller
    {
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(string s)
        {
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(string s)
        {
            return RedirectToAction("Login");
        }
    }
}
