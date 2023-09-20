using BusinessLayer.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CVSharer.Controllers
{
    [AllowAnonymous]
    public class CvController : Controller
    {
        private readonly IUserService _userService;

        public CvController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult BaseTemplate(string sharecode)
        {
            var user=_userService.GetUserByShareCode(sharecode);
            return View(user);
        }
        [HttpGet]
        public IActionResult Template2(string sharecode)
        {
			var user = _userService.GetUserByShareCode(sharecode);
			return View(user);
		}
        [HttpGet]
        public IActionResult Template3(string sharecode)
        {
			var user = _userService.GetUserByShareCode(sharecode);
			return View(user);
		}

        [HttpGet]
        public IActionResult Template4(string sharecode)
        {
            var user = _userService.GetUserByShareCode(sharecode);
            return View(user);
        }
    }
}
