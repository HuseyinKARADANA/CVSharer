using AspNetCoreHero.ToastNotification.Abstractions;
using BusinessLayer.Abstract;
using EntityLayer.Concrete;
using Humanizer;
using Microsoft.AspNetCore.Mvc;

namespace CVSharer.Controllers
{
    public class TemplateController : Controller
    {
        private readonly IUserService _userService;
        private readonly INotyfService _toast;


        public TemplateController(IUserService userService, INotyfService toast)
        {
            _userService = userService;
            _toast = toast;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult SelectTemplate(string template)
        {
            var userIdString = HttpContext.Request.Cookies["UserId"];
            int userId = int.Parse(userIdString);

            User userForUpdate = _userService.GetElementById(userId);

            userForUpdate.MainTemplate = template;

            _userService.Update(userForUpdate);

            _toast.Success("Template Selected");

            return RedirectToAction("Index");
        }
    }
}
