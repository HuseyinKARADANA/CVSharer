using Microsoft.AspNetCore.Mvc;

namespace CVSharer.Controllers
{
    public class ShopController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
