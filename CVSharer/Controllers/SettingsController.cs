using Microsoft.AspNetCore.Mvc;

namespace CVSharer.Controllers
{
	public class SettingsController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
