using BusinessLayer.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CVSharer.Controllers
{
	[AllowAnonymous]
    public class PdfController : Controller
    {
		private readonly IUserService _userService;

		public PdfController(IUserService userService)
		{
			_userService = userService;
		}

		public IActionResult BaseTemplate(string sharecode)
        {
			var user = _userService.GetUserByShareCode(sharecode);
			return View(user);
		}
        
        public IActionResult Template2(string sharecode)
        {
            var user = _userService.GetUserByShareCode(sharecode);
            return View(user);
        } 
        public IActionResult Template3(string sharecode)
        {
            var user = _userService.GetUserByShareCode(sharecode);
            return View(user);
        }
    }
}
