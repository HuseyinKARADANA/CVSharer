using AspNetCoreHero.ToastNotification.Abstractions;
using BusinessLayer.Abstract;
using CVSharer.Services;
using EntityLayer.Concrete;
using EntityLayer.DTOs;
using HtmlAgilityPack;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace CVSharer.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly PdfGenerator _pdfGenerator;
        private readonly IUserService _userService;
        private readonly INotyfService _toast;
        private readonly ISkillService _skillService;
        private readonly ICertificateService _certificateService;
        private readonly IEducationService _educationService;
        private readonly IExperienceService _experienceService;
        private readonly IHobbyService _hobbyService;
        private readonly ILanguageService _languageService;
        private readonly ILinkService _linkService;

        public DashboardController(PdfGenerator pdfGenerator, IUserService userService,INotyfService toast,ISkillService skillService,
            ICertificateService certificateService, IEducationService educationService, IExperienceService experienceService,
            IHobbyService hobbyService, ILanguageService languageService, ILinkService linkService)
        {
            _pdfGenerator = pdfGenerator;
            _userService = userService;
            _toast = toast;
            _skillService = skillService;
            _certificateService = certificateService;
            _educationService = educationService;
            _experienceService = experienceService;
            _hobbyService = hobbyService;
            _languageService = languageService;
            _linkService = linkService;
        }

        [HttpGet]
        public IActionResult Index(string? pages)
        {
            return View();
        }
        [HttpPost]
        public IActionResult DeleteProfile()
        {
            var usersId = HttpContext.Request.Cookies["UserId"];
            var UserId = int.Parse(usersId);
            User userForDeletePhoto = _userService.GetElementById(UserId);

            //veritabanı img silme-default resime geçiş
            userForDeletePhoto.Photo = "717ea7ab-aaf3-4081-89cb-51f4c8068308.png";
            _userService.Update(userForDeletePhoto);
            
         
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult AddProfile(UpdateUserDTO dto)
        {
            var userId = HttpContext.Request.Cookies["UserId"];
            dto.UserId = int.Parse(userId);

            User userForUpdate = _userService.GetElementById(dto.UserId);
            if (dto.Photo != null)
            {
                var extention = Path.GetExtension(dto.Photo.FileName);
                if(extention==".jpeg" || extention == ".png")
                {
					var newImageName = Guid.NewGuid() + extention;
					var location = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/ProfileImg/", newImageName);
					var stream = new FileStream(location, FileMode.Create);
					dto.Photo.CopyTo(stream);
					userForUpdate.Photo = newImageName;
					_userService.Update(userForUpdate);
				}
                else
                {
					
					_toast.Error("File format can only be '.jpeg' or '.png'!");
                }
                
            }
            

            return RedirectToAction("Index", "Dashboard");
        }
        [HttpPost]
        public IActionResult UpdateUser(UpdateUserDTO dto)
        {
            if(dto.Name == null)
            {
                _toast.Error("Name cannot be blank.");
                return RedirectToAction("Index", "Dashboard");
            }
            if (dto.Surname == null)
            {
                _toast.Error("Surname cannot be blank.");
                return RedirectToAction("Index", "Dashboard");
            }

            var userId = HttpContext.Request.Cookies["UserId"];
            dto.UserId = int.Parse(userId);

            User userForUpdate = _userService.GetElementById(dto.UserId);
            if(dto.Photo != null)
            {
                var extention=Path.GetExtension(dto.Photo.FileName);
                var newImageName = Guid.NewGuid() + extention;
                var location=Path.Combine(Directory.GetCurrentDirectory(),"wwwroot/ProfileImg/",newImageName);
                var stream=new FileStream(location, FileMode.Create);
                dto.Photo.CopyTo(stream);
                userForUpdate.Photo = newImageName;
            }
			
           
            userForUpdate.Name = dto.Name;
            userForUpdate.Surname = dto.Surname;
            userForUpdate.Description = dto.Description;
            userForUpdate.Position = dto.Position;
            userForUpdate.Phone = dto.Phone;
            userForUpdate.Address = dto.Address;
            userForUpdate.Linkedin = dto.Linkedin;
            userForUpdate.Instagram = dto.Instagram;
            userForUpdate.GitHub = dto.GitHub;
            userForUpdate.YouTube = dto.YouTube;

            _userService.Update(userForUpdate);

            return RedirectToAction("Index","Dashboard");
        }

        [HttpGet]
        public IActionResult AddSkill()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddSkill(Skill skill)
        {
            if(skill.SPercentage < 0 || skill.SPercentage > 100)
            {
                _toast.Error("Invalid Skill Percentage Value!");
                return View();
            }

            _skillService.Insert(new Skill()
            {
                UserId = skill.UserId,
                SName = skill.SName,
                SPercentage = skill.SPercentage,
            });

            return RedirectToAction("Index", "Dashboard");
        }

        [HttpGet]
        public IActionResult DeleteSkill(int skillId)
        {
            Skill skill = _skillService.GetElementById(skillId);

            if(skill == null)
            {
                _toast.Error("Skill Not Found!");
                return RedirectToAction("Index", "Dashboard");
            }

            _skillService.Delete(skill);

            _toast.Custom("Skill Deleted", 3, "orange", "bi bi-trash-fill");
            return RedirectToAction("Index","Dashboard");
        }


        [HttpPost]
        public async Task<IActionResult> DownloadPdf()
        {
            var userId = HttpContext.Request.Cookies["UserId"];
            var user = _userService.GetElementById(int.Parse(userId));

			var skills = _skillService.GetSkillsByUserId(int.Parse(userId));
			var certificates = _certificateService.GetCertificatesByUserId(int.Parse(userId));
			var educations = _educationService.GetEducationsByUserId(int.Parse(userId));
			var experiences = _experienceService.GetExperiencesByUserId(int.Parse(userId));
			var hobbies = _hobbyService.GetHobbiesByUserId(int.Parse(userId));
			var languages = _languageService.GetLanguagesByUserId(int.Parse(userId));
			var links = _linkService.GetLinksByUserId(int.Parse(userId));

			var fileName = user.Name + " " + user.Surname + " CV.pdf";
			string url = "https://www.cvsharer.com/Cv/BaseTemplate?shareCode=" + user.ShareCode;
            string htmlContent="";
			// HTTP isteği gönderin ve yanıtı alın
			using (HttpClient client = new HttpClient())
			{
			    htmlContent = await client.GetStringAsync(url);

				// HtmlAgilityPack ile HTML içeriğini ayrıştırın
				HtmlDocument htmlDocument = new HtmlDocument();
				htmlDocument.LoadHtml(htmlContent);

				// Sadece <body> içeriğini alın
				string bodyContent = htmlDocument.DocumentNode.SelectSingleNode("//body").InnerHtml;

				// bodyContent değişkeni şimdi sayfanın <body> içeriğini içeriyor
			}
			
            byte[] pdfBytes = _pdfGenerator.GeneratorPdf(htmlContent);

            return File(pdfBytes, "application/pdf", fileName);
        }
    }
}
