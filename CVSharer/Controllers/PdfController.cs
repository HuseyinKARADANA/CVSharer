using BusinessLayer.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace CVSharer.Controllers
{
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
    }
}
