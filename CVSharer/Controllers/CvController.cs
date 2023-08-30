using Microsoft.AspNetCore.Mvc;

namespace CVSharer.Controllers
{
    public class CvController : Controller
    {
        public IActionResult CV1()
        {
            return View();
        }
    }
}
