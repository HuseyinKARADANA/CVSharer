using Microsoft.AspNetCore.Mvc;

namespace CVSharer.Controllers
{
    public class TemplateController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
