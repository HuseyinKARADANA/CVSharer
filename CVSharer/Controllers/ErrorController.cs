using Microsoft.AspNetCore.Mvc;

namespace CVSharer.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Error401()
        {
            return View();
        }
    }
}
