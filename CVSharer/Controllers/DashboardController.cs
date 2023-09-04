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
                if(extention==".jpeg" || extention == ".png" || extention==".jpg")
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

        #region SkillCRUD
        //Skill CRUD Operations

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

            _toast.Success("Skill Added");

            return RedirectToAction("Index", "Dashboard");
        }

        [HttpGet]
        public IActionResult DeleteSkill(int skillId)
        {
            Skill skill = _skillService.GetElementById(skillId);

            var userIdString = HttpContext.Request.Cookies["UserId"];
            var userId = int.Parse(userIdString);

            if (userId != skill.UserId)
            {
                return RedirectToAction("Error401", "Error");
            }

            if(skill == null)
            {
                _toast.Error("Skill Not Found!");
                return RedirectToAction("Index", "Dashboard");
            }

            _skillService.Delete(skill);

            _toast.Custom("Skill Deleted", 3, "orange", "bi bi-trash-fill");
            return RedirectToAction("Index","Dashboard");
        }

        [HttpGet]
        public IActionResult UpdateSkill(int skillId)
        {
            var skill = _skillService.GetElementById(skillId);

            var userIdString = HttpContext.Request.Cookies["UserId"];
            var userId = int.Parse(userIdString);

            if (userId != skill.UserId)
            {
                return RedirectToAction("Error401", "Error");
            }

            return View(skill);
        }

        [HttpPost]
        public IActionResult UpdateSkill(Skill skill)
        {
            if (skill.SPercentage < 0 || skill.SPercentage > 100)
            {
                _toast.Error("Invalid Skill Percentage Value!");
                return View();
            }

            _skillService.Update(skill);

            _toast.Success("Skill Updated");

            return RedirectToAction("Index", "Dashboard");
        }

        #endregion

        #region HobbyCRUD
        //Hobby CRUD Operations

        [HttpGet]
        public IActionResult AddHobby()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddHobby(Hobby hobby)
        {
            _hobbyService.Insert(new Hobby()
            {
                UserId = hobby.UserId,
                HName = hobby.HName
            });

            _toast.Success("Hobby Added");

            return RedirectToAction("Index", "Dashboard");
        }

        [HttpGet]
        public IActionResult DeleteHobby(int hobbyId)
        {
            var hobby = _hobbyService.GetElementById(hobbyId);

            var userIdString = HttpContext.Request.Cookies["UserId"];
            var userId = int.Parse(userIdString);

            if (userId != hobby.UserId)
            {
                return RedirectToAction("Error401", "Error");
            }

            if (hobby == null)
            {
                _toast.Error("Hobby Not Found!");
                return RedirectToAction("Index", "Dashboard");
            }

            _hobbyService.Delete(hobby);

            _toast.Custom("Hobby Deleted", 3, "orange", "bi bi-trash-fill");
            return RedirectToAction("Index", "Dashboard");
        }

        [HttpGet]
        public IActionResult UpdateHobby(int hobbyId)
        {
            var hobby = _hobbyService.GetElementById(hobbyId);

            var userIdString = HttpContext.Request.Cookies["UserId"];
            var userId = int.Parse(userIdString);

            if (userId != hobby.UserId)
            {
                return RedirectToAction("Error401", "Error");
            }

            return View(hobby);
        }

        [HttpPost]
        public IActionResult UpdateHobby(Hobby hobby)
        {
            _hobbyService.Update(hobby);

            _toast.Success("Hobby Updated");

            return RedirectToAction("Index", "Dashboard");
        }
        #endregion

        #region LanguageCRUD
        //Language CRUD Operations

        [HttpGet]
        public IActionResult AddLanguage()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddLanguage(Language language)
        {
            if (language.LangPercentage < 0 || language.LangPercentage> 100)
            {
                _toast.Error("Invalid Language Percentage Value!");
                return View();
            }

            _languageService.Insert(new Language()
            {
                UserId = language.UserId,
                LangName = language.LangName,
                LangPercentage = language.LangPercentage,
            });

            _toast.Success("Language Added");

            return RedirectToAction("Index", "Dashboard");
        }

        [HttpGet]
        public IActionResult DeleteLanguage(int languageId)
        {
            Language language = _languageService.GetElementById(languageId);

            var userIdString = HttpContext.Request.Cookies["UserId"];
            var userId = int.Parse(userIdString);

            if (userId != language.UserId)
            {
                return RedirectToAction("Error401", "Error");
            }

            if (language == null)
            {
                _toast.Error("Language Not Found!");
                return RedirectToAction("Index", "Dashboard");
            }

            _languageService.Delete(language);

            _toast.Custom("Language Deleted", 3, "orange", "bi bi-trash-fill");
            return RedirectToAction("Index", "Dashboard");
        }

        [HttpGet]
        public IActionResult UpdateLanguage(int languageId)
        {
            var language = _languageService.GetElementById(languageId);

            var userIdString = HttpContext.Request.Cookies["UserId"];
            var userId = int.Parse(userIdString);

            if (userId != language.UserId)
            {
                return RedirectToAction("Error401", "Error");
            }

            return View(language);
        }

        [HttpPost]
        public IActionResult UpdateLanguage(Language language)
        {
            if (language.LangPercentage< 0 || language.LangPercentage> 100)
            {
                _toast.Error("Invalid Language Percentage Value!");
                return View();
            }

            _languageService.Update(language);
            _languageService.Update(language);

            _toast.Success("Language Updated");

            return RedirectToAction("Index", "Dashboard");
        }

        #endregion

        #region CerticifateCRUD
        //Certificate CRUD Operations

        [HttpGet]
        public IActionResult AddCertificate()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddCertificate(Certificate certificate)
        {
            _certificateService.Insert(new Certificate()
            {
                UserId = certificate.UserId,
                CName = certificate.CName,
                Institution = certificate.Institution,
                Url = certificate.Url,
                StartDate = certificate.StartDate,
                EndDate = certificate.EndDate,
            });

            _toast.Success("Certificate Added");

            return RedirectToAction("Index", "Dashboard");
        }

        [HttpGet]
        public IActionResult DeleteCertificate(int certificateId)
        {
            var certificate = _certificateService.GetElementById(certificateId);

            var userIdString = HttpContext.Request.Cookies["UserId"];
            var userId = int.Parse(userIdString);

            if (userId != certificate.UserId)
            {
                return RedirectToAction("Error401", "Error");
            }

            if (certificate == null)
            {
                _toast.Error("Certificate Not Found!");
                return RedirectToAction("Index", "Dashboard");
            }

            _certificateService.Delete(certificate);

            _toast.Custom("Certificate Deleted", 3, "orange", "bi bi-trash-fill");
            return RedirectToAction("Index", "Dashboard");
        }

        [HttpGet]
        public IActionResult UpdateCertificate(int certificateId)
        {
            var certificate = _certificateService.GetElementById(certificateId);

            var userIdString = HttpContext.Request.Cookies["UserId"];
            var userId = int.Parse(userIdString);

            if (userId != certificate.UserId)
            {
                return RedirectToAction("Error401", "Error");
            }

            return View(certificate);
        }

        [HttpPost]
        public IActionResult UpdateCertificate(Certificate certificate)
        {
            _certificateService.Update(certificate);

            _toast.Success("Certificate Updated");

            return RedirectToAction("Index", "Dashboard");
        }
        #endregion

        #region EducationCRUD
        //Education CRUD Operations

        [HttpGet]
        public IActionResult AddEducation()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddEducation(Education education)
        {
            _educationService.Insert(new Education()
            {
                UserId = education.UserId,
                EName = education.EName,
                StartDate = education.StartDate,
                EndDate = education.EndDate,
            });

            _toast.Success("Education Added");

            return RedirectToAction("Index", "Dashboard");
        }

        [HttpGet]
        public IActionResult DeleteEducation(int educationId)
        {
            var education = _educationService.GetElementById(educationId);

            var userIdString = HttpContext.Request.Cookies["UserId"];
            var userId = int.Parse(userIdString);

            if (userId != education.UserId)
            {
                return RedirectToAction("Error401", "Error");
            }

            if (education == null)
            {
                _toast.Error("Education Not Found!");
                return RedirectToAction("Index", "Dashboard");
            }

            _educationService.Delete(education);

            _toast.Custom("Education Deleted", 3, "orange", "bi bi-trash-fill");
            return RedirectToAction("Index", "Dashboard");
        }

        [HttpGet]
        public IActionResult UpdateEducation(int educationId)
        {
            var education = _educationService.GetElementById(educationId);

            var userIdString = HttpContext.Request.Cookies["UserId"];
            var userId = int.Parse(userIdString);

            if (userId != education.UserId)
            {
                return RedirectToAction("Error401", "Error");
            }

            return View(education);
        }

        [HttpPost]
        public IActionResult UpdateEducation(Education education)
        {
            _educationService.Update(education);

            _toast.Success("Education Updated");

            return RedirectToAction("Index", "Dashboard");
        }
        #endregion

        #region ExperienceCRUD
        //Experience CRUD Operations

        [HttpGet]
        public IActionResult AddExperience()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddExperience(Experience experience)
        {
            _experienceService.Insert(new Experience()
            {
                UserId = experience.UserId,
                ExName = experience.ExName,
                StartDate = experience.StartDate,
                EndDate = experience.EndDate,
            });

            _toast.Success("Experience Added");

            return RedirectToAction("Index", "Dashboard");
        }

        [HttpGet]
        public IActionResult DeleteExperience(int experienceId)
        {
            var experience = _experienceService.GetElementById(experienceId);

            var userIdString = HttpContext.Request.Cookies["UserId"];
            var userId = int.Parse(userIdString);

            if (userId != experience.UserId)
            {
                return RedirectToAction("Error401", "Error");
            }

            if (experience == null)
            {
                _toast.Error("Experience Not Found!");
                return RedirectToAction("Index", "Dashboard");
            }

            _experienceService.Delete(experience);

            _toast.Custom("Experience Deleted", 3, "orange", "bi bi-trash-fill");
            return RedirectToAction("Index", "Dashboard");
        }

        [HttpGet]
        public IActionResult UpdateExperience(int experienceId)
        {
            var experience = _experienceService.GetElementById(experienceId);

            var userIdString = HttpContext.Request.Cookies["UserId"];
            var userId = int.Parse(userIdString);

            if (userId != experience.UserId)
            {
                return RedirectToAction("Error401", "Error");
            }

            return View(experience);
        }

        [HttpPost]
        public IActionResult UpdateExperience(Experience experience)
        {
            _experienceService.Update(experience);

            _toast.Success("Experience Updated");

            return RedirectToAction("Index", "Dashboard");
        }
        #endregion

        #region LinkCRUD
        //Link CRUD Operations

        [HttpGet]
        public IActionResult AddLink()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddLink(Link link)
        {
            _linkService.Insert(new Link()
            {
                UserId = link.UserId,
                LName = link.LName,
                LUrl = link.LUrl,
            });

            _toast.Success("Link Added");

            return RedirectToAction("Index", "Dashboard");
        }

        [HttpGet]
        public IActionResult DeleteLink(int linkId)
        {
            var link = _linkService.GetElementById(linkId);

            var userIdString = HttpContext.Request.Cookies["UserId"];
            var userId = int.Parse(userIdString);

            if (userId != link.UserId)
            {
                return RedirectToAction("Error401", "Error");
            }

            if (link == null)
            {
                _toast.Error("Link Not Found!");
                return RedirectToAction("Index", "Dashboard");
            }

            _linkService.Delete(link);

            _toast.Custom("Link Deleted", 3, "orange", "bi bi-trash-fill");
            return RedirectToAction("Index", "Dashboard");
        }

        [HttpGet]
        public IActionResult UpdateLink(int linkId)
        {
            var link = _linkService.GetElementById(linkId);

            var userIdString = HttpContext.Request.Cookies["UserId"];
            var userId = int.Parse(userIdString);

            if (userId != link.UserId)
            {
                return RedirectToAction("Error401", "Error");
            }

            return View(link);
        }

        [HttpPost]
        public IActionResult UpdateLink(Link link)
        {
            _linkService.Update(link);

            _toast.Success("Link Updated");

            return RedirectToAction("Index", "Dashboard");
        }
        #endregion

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> DownloadPdf(string shareCode)
        {
            var user = _userService.GetUserByShareCode(shareCode);
            var userId = user.UserId;

			var skills = _skillService.GetSkillsByUserId(userId);
			var certificates = _certificateService.GetCertificatesByUserId(userId);
			var educations = _educationService.GetEducationsByUserId(userId);
			var experiences = _experienceService.GetExperiencesByUserId(userId);
			var hobbies = _hobbyService.GetHobbiesByUserId(userId);
			var languages = _languageService.GetLanguagesByUserId(userId);
			var links = _linkService.GetLinksByUserId(userId);

			var fileName = user.Name + " " + user.Surname + " CV.pdf";
			string url = "https://www.cvsharer.com/Pdf/BaseTemplate?ShareCode=" + user.ShareCode;
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
