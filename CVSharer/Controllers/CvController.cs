using Microsoft.AspNetCore.Mvc;

namespace CVSharer.Controllers
{
    public class CvController : Controller
    {
        [HttpGet]
        public IActionResult BaseTemplate()
        {
            return View();
        }
    }
}
