using AspNetCoreHero.ToastNotification.Abstractions;
using BusinessLayer.Abstract;
using CVSharer.Services;
using EntityLayer.Concrete;
using EntityLayer.DTOs;
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
                var newImageName = Guid.NewGuid() + extention;
                var location = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/ProfileImg/", newImageName);
                var stream = new FileStream(location, FileMode.Create);
                dto.Photo.CopyTo(stream);
                userForUpdate.Photo = newImageName;
            }
            _userService.Update(userForUpdate);

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

        [HttpPost]
        public IActionResult DownloadPdf()
        {
            var userId = HttpContext.Request.Cookies["UserId"];
            var user = _userService.GetElementById(int.Parse(userId));
            var fileName = user.Name + " " + user.Surname + " CV.pdf";

            string htmlContent = "<html>\r\n\t\t\t\t<head>\r\n\t\t\t\t\t<style>\r\n\t\t\t\t\t\t* {\r\n\t\t\t\t\t\t\tbox-sizing: border-box;\r\n\t\t\t\t\t\t}\r\n\r\n\t\t\t\t\t\tbody {\r\n\t\t\t\t\t\t\tmargin: 0;\r\n\t\t\t\t\t\t\tpadding: 0;\r\n\t\t\t\t\t\t}\r\n\r\n\t\t\t\t\t\ta[x-apple-data-detectors] {\r\n\t\t\t\t\t\t\tcolor: inherit !important;\r\n\t\t\t\t\t\t\ttext-decoration: inherit !important;\r\n\t\t\t\t\t\t}\r\n\r\n\t\t\t\t\t\t#MessageViewBody a {\r\n\t\t\t\t\t\t\tcolor: inherit;\r\n\t\t\t\t\t\t\ttext-decoration: none;\r\n\t\t\t\t\t\t}\r\n\r\n\t\t\t\t\t\tp {\r\n\t\t\t\t\t\t\tline-height: inherit\r\n\t\t\t\t\t\t}\r\n\r\n\t\t\t\t\t\t.desktop_hide,\r\n\t\t\t\t\t\t.desktop_hide table {\r\n\t\t\t\t\t\t\tmso-hide: all;\r\n\t\t\t\t\t\t\tdisplay: none;\r\n\t\t\t\t\t\t\tmax-height: 0px;\r\n\t\t\t\t\t\t\toverflow: hidden;\r\n\t\t\t\t\t\t}\r\n\r\n\t\t\t\t\t\t.image_block img + div {\r\n\t\t\t\t\t\t\tdisplay: none;\r\n\t\t\t\t\t\t}\r\n\r\n\t\t\t\t\t</style>\r\n\t\t\t\t</head>\r\n\r\n\t\t\t\t<body style='margin: 0; padding: 0; -webkit-text-size-adjust: none; text-size-adjust: none; background-color: #f4f1eb;'>\r\n\t\t\t\t\t<table class='nl-container' width='100%' border='0' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-color: #f4f1eb;'>\r\n\t\t\t\t\t\t<tbody>\r\n\t\t\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t\t\t<td>\r\n\t\t\t\t\t\t\t\t\t<table class='row row-1' align='center' width='100%' border='0' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;'>\r\n\t\t\t\t\t\t\t\t\t\t<tbody>\r\n\t\t\t\t\t\t\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t<td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t<table class='row-content stack' align='center' border='0' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-color: #f4f1eb; color: #000; width: 680px; margin: 0 auto;' width='680'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t<tbody>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<td class='column column-1' width='100%' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; vertical-align: top; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<div class='spacer_block block-1' style='height:20px;line-height:20px;font-size:1px;'>&#8202;</div>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t</tbody>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t</table>\r\n\t\t\t\t\t\t\t\t\t\t\t\t</td>\r\n\t\t\t\t\t\t\t\t\t\t\t</tr>\r\n\t\t\t\t\t\t\t\t\t\t</tbody>\r\n\t\t\t\t\t\t\t\t\t</table>\r\n\t\t\t\t\t\t\t\t\t<table class='row row-2' align='center' width='100%' border='0' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;'>\r\n\t\t\t\t\t\t\t\t\t\t<tbody>\r\n\t\t\t\t\t\t\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t<td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t<table class='row-content stack' align='center' border='0' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-color: #f4f1eb; color: #000; width: 680px; margin: 0 auto;' width='680'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t<tbody>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<td class='column column-1' width='33.333333333333336%' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; vertical-align: top; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<table class='empty_block block-1' width='100%' border='0' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<td class='pad'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<div></div>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</table>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<td class='column column-2' width='33.333333333333336%' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; padding-bottom: 5px; padding-top: 5px; vertical-align: top; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<table class='empty_block block-1' width='100%' border='0' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<td class='pad'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<div></div>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</table>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<td class='column column-3' width='33.333333333333336%' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; padding-bottom: 5px; padding-top: 5px; vertical-align: top; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<table class='image_block block-1' width='100%' border='0' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<td class='pad' style='padding-bottom:10px;width:100%;padding-right:0px;padding-left:0px;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<div class='alignment' align='center' style='line-height:10px'><img src='https://f432a44714.imgdist.com/public/users/Integrators/BeeProAgency/1046008_1031170/logo.png' style='display: block; height: auto; border: 0; max-width: 68px; width: 100%;' width='68'></div>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</table>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t</tbody>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t</table>\r\n\t\t\t\t\t\t\t\t\t\t\t\t</td>\r\n\t\t\t\t\t\t\t\t\t\t\t</tr>\r\n\t\t\t\t\t\t\t\t\t\t</tbody>\r\n\t\t\t\t\t\t\t\t\t</table>\r\n\t\t\t\t\t\t\t\t\t<table class='row row-3' align='center' width='100%' border='0' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;'>\r\n\t\t\t\t\t\t\t\t\t\t<tbody>\r\n\t\t\t\t\t\t\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t<td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t<table class='row-content stack' align='center' border='0' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-color: #f4f1eb; border-radius: 0; color: #000; width: 680px; margin: 0 auto;' width='680'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t<tbody>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<td class='column column-1' width='33.333333333333336%' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; vertical-align: top; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<table class='image_block block-1' width='100%' border='0' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<td class='pad' style='width:100%;padding-right:0px;padding-left:0px;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<div class='alignment' align='center' style='line-height:10px'><img src='https://d1oco4z2z1fhwp.cloudfront.net/templates/default/7331/woman-10.png' style='display: block; height: auto; border: 0; max-width: 226.66666666666666px; width: 100%;' width='226.66666666666666' alt='Woman image' title='Woman image'></div>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</table>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<td class='column column-2' width='66.66666666666667%' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; padding-bottom: 20px; padding-left: 20px; padding-right: 20px; padding-top: 20px; vertical-align: top; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<table class='paragraph_block block-1' width='100%' border='0' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; word-break: break-word;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<td class='pad' style='padding-bottom:10px;padding-left:20px;padding-right:20px;padding-top:10px;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<div style='color:#784a3b;direction:ltr;font-family:Georgia, Times, ' Times New Roman', serif;font-size:35px;font-weight:400;letter-spacing:0px;line-height:120%;text-align:left;mso-line-height-alt:42px;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<p style='margin: 0;'><em>Emma Watson</em></p>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</div>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</table>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<table class='paragraph_block block-2' width='100%' border='0' cellpadding='10' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; word-break: break-word;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<td class='pad'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<div style='color:#d56533;direction:ltr;font-family:Arial, Helvetica Neue, Helvetica, sans-serif;font-size:20px;font-weight:400;letter-spacing:5px;line-height:120%;text-align:left;mso-line-height-alt:24px;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<p style='margin: 0;'>Software Engineer</p>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</div>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</table>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<div class='spacer_block block-3' style='height:20px;line-height:20px;font-size:1px;'>&#8202;</div>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<table class='paragraph_block block-4' width='100%' border='0' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; word-break: break-word;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<td class='pad' style='padding-top:10px;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<div style='color:#393d47;direction:ltr;font-family:Arial, Helvetica Neue, Helvetica, sans-serif;font-size:14px;font-weight:400;letter-spacing:0px;line-height:150%;text-align:left;mso-line-height-alt:21px;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<p style='margin: 0;'>&nbsp; &nbsp;Ben Emma Watson 20 yaşındayım. İzmir'de yaşıyorum. Yazılım mühendisliği üzerine lisans okuyorum. Kendimi farklı alanlarda geliştiriyorum. Şuanda java, C# ve ASP.NET teknolojileri üzerinde orta/ortanın bir seviye üstü bilgim olduğunu düşünüyorum. Veritabanı olarak MySQL, MsSQL ve Access üzerinde deneyimim bulunuyor. Araştırmayı ve projeler yapmayı seviyorum.&nbsp;</p>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</div>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</table>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t</tbody>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t</table>\r\n\t\t\t\t\t\t\t\t\t\t\t\t</td>\r\n\t\t\t\t\t\t\t\t\t\t\t</tr>\r\n\t\t\t\t\t\t\t\t\t\t</tbody>\r\n\t\t\t\t\t\t\t\t\t</table>\r\n\t\t\t\t\t\t\t\t\t<table class='row row-4' align='center' width='100%' border='0' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;'>\r\n\t\t\t\t\t\t\t\t\t\t<tbody>\r\n\t\t\t\t\t\t\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t<td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t<table class='row-content stack' align='center' border='0' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-color: #f4f1eb; border-radius: 0; color: #000; width: 680px; margin: 0 auto;' width='680'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t<tbody>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<td class='column column-1' width='33.333333333333336%' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; background-color: #ffd188; vertical-align: top; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<table class='paragraph_block block-1' width='100%' border='0' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; word-break: break-word;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<td class='pad' style='padding-bottom:10px;padding-left:10px;padding-right:10px;padding-top:15px;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<div style='color:#101112;direction:ltr;font-family:Arial, Helvetica Neue, Helvetica, sans-serif;font-size:16px;font-weight:400;letter-spacing:0px;line-height:120%;text-align:center;mso-line-height-alt:19.2px;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<p style='margin: 0;'><strong>Personal Info</strong></p>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</div>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</table>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<table class='divider_block block-2' width='100%' border='0' cellpadding='10' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<td class='pad'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<div class='alignment' align='center'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<table border='0' cellpadding='0' cellspacing='0' role='presentation' width='75%' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<td class='divider_inner' style='font-size: 1px; line-height: 1px; border-top: 2px solid #000000;'><span>&#8202;</span></td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</table>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</div>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</table>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<table class='paragraph_block block-3' width='100%' border='0' cellpadding='10' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; word-break: break-word;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<td class='pad'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<div style='color:#101112;direction:ltr;font-family:Arial, Helvetica Neue, Helvetica, sans-serif;font-size:14px;font-weight:400;letter-spacing:0px;line-height:120%;text-align:left;mso-line-height-alt:16.8px;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<p style='margin: 0; margin-bottom: 16px;'><strong>Phone:</strong></p>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<p style='margin: 0; margin-bottom: 16px;'><strong> </strong>5467872570</p>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<p style='margin: 0; margin-bottom: 16px;'><strong>Email:</strong></p>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<p style='margin: 0;'><span style='font-family: inherit;'>huseyinkaradana35@gmail.com</span></p>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</div>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</table>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<table class='paragraph_block block-4' width='100%' border='0' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; word-break: break-word;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<td class='pad' style='padding-bottom:10px;padding-left:10px;padding-right:10px;padding-top:15px;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<div style='color:#101112;direction:ltr;font-family:Arial, Helvetica Neue, Helvetica, sans-serif;font-size:16px;font-weight:400;letter-spacing:0px;line-height:120%;text-align:center;mso-line-height-alt:19.2px;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<p style='margin: 0;'><strong>Languages</strong></p>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</div>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</table>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<table class='divider_block block-5' width='100%' border='0' cellpadding='10' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<td class='pad'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<div class='alignment' align='center'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<table border='0' cellpadding='0' cellspacing='0' role='presentation' width='75%' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<td class='divider_inner' style='font-size: 1px; line-height: 1px; border-top: 2px solid #000000;'><span>&#8202;</span></td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</table>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</div>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</table><!--[if mso]><style>#list-r3c0m5 ul{margin: 0 !important; padding: 0 !important;} #list-r3c0m5 ul li{mso-special-format: bullet;}#list-r3c0m5 .levelOne li {margin-top: 0 !important;} #list-r3c0m5 .levelOne {margin-left: -20px !important;}#list-r3c0m5 .levelTwo li {margin-top: 0 !important;} #list-r3c0m5 .levelTwo {margin-left: 10px !important;}#list-r3c0m5 .levelThree li {margin-top: 0 !important;} #list-r3c0m5 .levelThree {margin-left: 40px !important;}</style><![endif]-->\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<table class='list_block block-6' id='list-r3c0m5' width='100%' border='0' cellpadding='10' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; word-break: break-word;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<td class='pad'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<div class='levelOne' style='margin-left: 0;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<ul class='leftList' start='1' style='margin-top: 0; margin-bottom: 0; padding: 0; padding-left: 20px; color: #101112; direction: ltr; font-family: Arial,Helvetica Neue,Helvetica,sans-serif; font-size: 16px; font-weight: 400; letter-spacing: 0; line-height: 120%; text-align: left; mso-line-height-alt: 19.2px; list-style-type: disc;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<li style='margin-bottom: 0; text-align: left;'>English</li>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<li style='margin-bottom: 0; text-align: left;'>Turkish</li>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</ul>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</div>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</table>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<td class='column column-2' width='66.66666666666667%' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; padding-bottom: 5px; padding-top: 5px; vertical-align: top; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<table class='heading_block block-1' width='100%' border='0' cellpadding='10' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<td class='pad'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<h3 style='margin: 0; color: #41413c; direction: ltr; font-family: Arial, Helvetica Neue, Helvetica, sans-serif; font-size: 16px; font-weight: 700; letter-spacing: normal; line-height: 120%; text-align: center; margin-top: 0; margin-bottom: 0;'><span class='tinyMce-placeholder'>Education</span></h3>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</table>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<table class='divider_block block-2' width='100%' border='0' cellpadding='10' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<td class='pad'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<div class='alignment' align='center'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<table border='0' cellpadding='0' cellspacing='0' role='presentation' width='75%' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<td class='divider_inner' style='font-size: 1px; line-height: 1px; border-top: 2px solid #000000;'><span>&#8202;</span></td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</table>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</div>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</table>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<table class='paragraph_block block-3' width='100%' border='0' cellpadding='10' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; word-break: break-word;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<td class='pad'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<div style='color:#101112;direction:ltr;font-family:Arial, Helvetica Neue, Helvetica, sans-serif;font-size:16px;font-weight:400;letter-spacing:0px;line-height:120%;text-align:left;mso-line-height-alt:19.2px;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<p style='margin: 0; margin-bottom: 16px;'><strong>2021-2025</strong></p>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<p style='margin: 0; margin-bottom: 16px;'>Mehmet Akif Ersoy University-Software Engineering Licence</p>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<p style='margin: 0; margin-bottom: 16px;'>&nbsp;</p>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<p style='margin: 0; margin-bottom: 16px;'><strong>2025-2029</strong></p>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<p style='margin: 0; margin-bottom: 16px;'>Gazi University-Software Engineering Hight Licence</p>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<p style='margin: 0;'>&nbsp;</p>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</div>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</table>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t</tbody>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t</table>\r\n\t\t\t\t\t\t\t\t\t\t\t\t</td>\r\n\t\t\t\t\t\t\t\t\t\t\t</tr>\r\n\t\t\t\t\t\t\t\t\t\t</tbody>\r\n\t\t\t\t\t\t\t\t\t</table>\r\n\t\t\t\t\t\t\t\t\t<table class='row row-5' align='center' width='100%' border='0' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;'>\r\n\t\t\t\t\t\t\t\t\t\t<tbody>\r\n\t\t\t\t\t\t\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t<td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t<table class='row-content stack' align='center' border='0' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-color: #f4f1eb; border-radius: 0; color: #000; width: 680px; margin: 0 auto;' width='680'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t<tbody>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<td class='column column-1' width='33.333333333333336%' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; background-color: #ffd188; vertical-align: top; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<table class='paragraph_block block-1' width='100%' border='0' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; word-break: break-word;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<td class='pad' style='padding-bottom:10px;padding-left:10px;padding-right:10px;padding-top:15px;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<div style='color:#101112;direction:ltr;font-family:Arial, Helvetica Neue, Helvetica, sans-serif;font-size:16px;font-weight:400;letter-spacing:0px;line-height:120%;text-align:center;mso-line-height-alt:19.2px;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<p style='margin: 0;'><strong>Hobbies</strong></p>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</div>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</table>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<table class='divider_block block-2' width='100%' border='0' cellpadding='10' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<td class='pad'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<div class='alignment' align='center'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<table border='0' cellpadding='0' cellspacing='0' role='presentation' width='75%' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<td class='divider_inner' style='font-size: 1px; line-height: 1px; border-top: 2px solid #000000;'><span>&#8202;</span></td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</table>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</div>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</table><!--[if mso]><style>#list-r4c0m2 ul{margin: 0 !important; padding: 0 !important;} #list-r4c0m2 ul li{mso-special-format: bullet;}#list-r4c0m2 .levelOne li {margin-top: 0 !important;} #list-r4c0m2 .levelOne {margin-left: -20px !important;}#list-r4c0m2 .levelTwo li {margin-top: 0 !important;} #list-r4c0m2 .levelTwo {margin-left: 10px !important;}#list-r4c0m2 .levelThree li {margin-top: 0 !important;} #list-r4c0m2 .levelThree {margin-left: 40px !important;}</style><![endif]-->\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<table class='list_block block-3' id='list-r4c0m2' width='100%' border='0' cellpadding='10' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; word-break: break-word;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<td class='pad'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<div class='levelOne' style='margin-left: 0;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<ul class='leftList' start='1' style='margin-top: 0; margin-bottom: 0; padding: 0; padding-left: 20px; color: #101112; direction: ltr; font-family: Arial,Helvetica Neue,Helvetica,sans-serif; font-size: 16px; font-weight: 400; letter-spacing: 0; line-height: 120%; text-align: left; mso-line-height-alt: 19.2px; list-style-type: disc;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<li style='margin-bottom: 0; text-align: left;'>Play Game</li>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<li style='margin-bottom: 0; text-align: left;'>Searching new technologies</li>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<li style='margin-bottom: 0; text-align: left;'>Play Computer Games</li>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</ul>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</div>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</table>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<td class='column column-2' width='66.66666666666667%' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; padding-bottom: 5px; padding-top: 5px; vertical-align: top; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<table class='heading_block block-1' width='100%' border='0' cellpadding='10' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<td class='pad'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<h1 style='margin: 0; color: #41413c; direction: ltr; font-family: Arial, Helvetica Neue, Helvetica, sans-serif; font-size: 16px; font-weight: 700; letter-spacing: normal; line-height: 120%; text-align: center; margin-top: 0; margin-bottom: 0;'><span class='tinyMce-placeholder'>Experience</span></h1>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</table>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<table class='divider_block block-2' width='100%' border='0' cellpadding='10' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<td class='pad'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<div class='alignment' align='center'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<table border='0' cellpadding='0' cellspacing='0' role='presentation' width='75%' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<td class='divider_inner' style='font-size: 1px; line-height: 1px; border-top: 2px solid #000000;'><span>&#8202;</span></td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</table>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</div>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</table>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<table class='paragraph_block block-3' width='100%' border='0' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; word-break: break-word;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<td class='pad' style='padding-left:10px;padding-right:10px;padding-top:10px;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<div style='color:#101112;direction:ltr;font-family:Arial, Helvetica Neue, Helvetica, sans-serif;font-size:16px;font-weight:400;letter-spacing:0px;line-height:120%;text-align:left;mso-line-height-alt:19.2px;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<p style='margin: 0; margin-bottom: 16px;'><strong>2021-2025</strong></p>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<p style='margin: 0; margin-bottom: 16px;'>Monovi BT- Application Development Department</p>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<p style='margin: 0;'>&nbsp;</p>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</div>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</table>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t</tbody>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t</table>\r\n\t\t\t\t\t\t\t\t\t\t\t\t</td>\r\n\t\t\t\t\t\t\t\t\t\t\t</tr>\r\n\t\t\t\t\t\t\t\t\t\t</tbody>\r\n\t\t\t\t\t\t\t\t\t</table>\r\n\t\t\t\t\t\t\t\t\t<table class='row row-6' align='center' width='100%' border='0' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;'>\r\n\t\t\t\t\t\t\t\t\t\t<tbody>\r\n\t\t\t\t\t\t\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t<td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t<table class='row-content stack' align='center' border='0' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-color: #f4f1eb; color: #000; width: 680px; margin: 0 auto;' width='680'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t<tbody>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<td class='column column-1' width='33.333333333333336%' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; background-color: #ffd188; vertical-align: top; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<table class='paragraph_block block-1' width='100%' border='0' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; word-break: break-word;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<td class='pad' style='padding-bottom:10px;padding-left:10px;padding-right:10px;padding-top:15px;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<div style='color:#101112;direction:ltr;font-family:Arial, Helvetica Neue, Helvetica, sans-serif;font-size:16px;font-weight:400;letter-spacing:0px;line-height:120%;text-align:center;mso-line-height-alt:19.2px;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<p style='margin: 0;'><strong>Skills</strong></p>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</div>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</table>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<table class='divider_block block-2' width='100%' border='0' cellpadding='10' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<td class='pad'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<div class='alignment' align='center'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<table border='0' cellpadding='0' cellspacing='0' role='presentation' width='75%' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<td class='divider_inner' style='font-size: 1px; line-height: 1px; border-top: 2px solid #000000;'><span>&#8202;</span></td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</table>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</div>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</table><!--[if mso]><style>#list-r5c0m2 ul{margin: 0 !important; padding: 0 !important;} #list-r5c0m2 ul li{mso-special-format: bullet;}#list-r5c0m2 .levelOne li {margin-top: 0 !important;} #list-r5c0m2 .levelOne {margin-left: -20px !important;}#list-r5c0m2 .levelTwo li {margin-top: 0 !important;} #list-r5c0m2 .levelTwo {margin-left: 10px !important;}#list-r5c0m2 .levelThree li {margin-top: 0 !important;} #list-r5c0m2 .levelThree {margin-left: 40px !important;}</style><![endif]-->\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<table class='list_block block-3' id='list-r5c0m2' width='100%' border='0' cellpadding='10' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; word-break: break-word;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<td class='pad'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<div class='levelOne' style='margin-left: 0;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<ul class='leftList' start='1' style='margin-top: 0; margin-bottom: 0; padding: 0; padding-left: 20px; color: #101112; direction: ltr; font-family: Arial,Helvetica Neue,Helvetica,sans-serif; font-size: 16px; font-weight: 400; letter-spacing: 0; line-height: 120%; text-align: left; mso-line-height-alt: 19.2px; list-style-type: disc;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<li style='margin-bottom: 0; text-align: left;'>Java</li>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<li style='margin-bottom: 0; text-align: left;'>C#</li>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<li style='margin-bottom: 0; text-align: left;'>ASP.NET</li>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<li style='margin-bottom: 0; text-align: left;'>MsSQL</li>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</ul>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</div>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</table>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<td class='column column-2' width='66.66666666666667%' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; padding-bottom: 5px; padding-top: 5px; vertical-align: top; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<table class='heading_block block-1' width='100%' border='0' cellpadding='10' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<td class='pad'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<h1 style='margin: 0; color: #41413c; direction: ltr; font-family: Arial, Helvetica Neue, Helvetica, sans-serif; font-size: 16px; font-weight: 700; letter-spacing: normal; line-height: 120%; text-align: center; margin-top: 0; margin-bottom: 0;'><span class='tinyMce-placeholder'>Contact</span></h1>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</table>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<table class='divider_block block-2' width='100%' border='0' cellpadding='10' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<td class='pad'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<div class='alignment' align='center'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<table border='0' cellpadding='0' cellspacing='0' role='presentation' width='75%' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<td class='divider_inner' style='font-size: 1px; line-height: 1px; border-top: 2px solid #000000;'><span>&#8202;</span></td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</table>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</div>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</table>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<table class='paragraph_block block-3' width='100%' border='0' cellpadding='10' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; word-break: break-word;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<td class='pad'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<div style='color:#101112;direction:ltr;font-family:Arial, Helvetica Neue, Helvetica, sans-serif;font-size:16px;font-weight:400;letter-spacing:0px;line-height:120%;text-align:left;mso-line-height-alt:19.2px;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<p style='margin: 0; margin-bottom: 16px;'><a href='https://www.linkedin.com/in/huseyin-karadana/' target='_blank' style='text-decoration: underline; color: #7747FF;' rel='noopener'>https://www.linkedin.com/in/huseyin-karadana/</a></p>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<p style='margin: 0; margin-bottom: 16px;'><a href='https://github.com/HuseyinKARADANA' target='_blank' style='text-decoration: underline; color: #7747FF;' rel='noopener'>https://github.com/HuseyinKARADANA</a></p>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<p style='margin: 0; margin-bottom: 16px;'>&nbsp;</p>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<p style='margin: 0;'>&nbsp;</p>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</div>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</table>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t</tbody>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t</table>\r\n\t\t\t\t\t\t\t\t\t\t\t\t</td>\r\n\t\t\t\t\t\t\t\t\t\t\t</tr>\r\n\t\t\t\t\t\t\t\t\t\t</tbody>\r\n\t\t\t\t\t\t\t\t\t</table>\r\n\t\t\t\t\t\t\t\t\t<table class='row row-7' align='center' width='100%' border='0' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;'>\r\n\t\t\t\t\t\t\t\t\t\t<tbody>\r\n\t\t\t\t\t\t\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t<td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t<table class='row-content stack' align='center' border='0' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-color: #f4f1eb; color: #000; width: 680px; margin: 0 auto;' width='680'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t<tbody>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<td class='column column-1' width='33.333333333333336%' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; background-color: #ffd188; vertical-align: top; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<table class='paragraph_block block-1' width='100%' border='0' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; word-break: break-word;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<td class='pad' style='padding-bottom:10px;padding-left:10px;padding-right:10px;padding-top:15px;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<div style='color:#101112;direction:ltr;font-family:Arial, Helvetica Neue, Helvetica, sans-serif;font-size:16px;font-weight:400;letter-spacing:0px;line-height:120%;text-align:center;mso-line-height-alt:19.2px;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<p style='margin: 0;'><strong>Address</strong></p>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</div>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</table>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<table class='divider_block block-2' width='100%' border='0' cellpadding='10' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<td class='pad'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<div class='alignment' align='center'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<table border='0' cellpadding='0' cellspacing='0' role='presentation' width='75%' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<td class='divider_inner' style='font-size: 1px; line-height: 1px; border-top: 2px solid #000000;'><span>&#8202;</span></td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</table>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</div>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</table>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<table class='paragraph_block block-3' width='100%' border='0' cellpadding='10' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; word-break: break-word;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<td class='pad'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<div style='color:#101112;direction:ltr;font-family:Arial, Helvetica Neue, Helvetica, sans-serif;font-size:14px;font-weight:400;letter-spacing:0px;line-height:120%;text-align:left;mso-line-height-alt:16.8px;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<p style='margin: 0;'>İzmir Güzelbahçe Hacı yatmaz mah. 10_12 daire no:13</p>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</div>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</table>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<td class='column column-2' width='66.66666666666667%' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; padding-bottom: 5px; padding-top: 5px; vertical-align: top; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<table class='heading_block block-1' width='100%' border='0' cellpadding='10' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<td class='pad'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<h1 style='margin: 0; color: #41413c; direction: ltr; font-family: Arial, Helvetica Neue, Helvetica, sans-serif; font-size: 16px; font-weight: 700; letter-spacing: normal; line-height: 120%; text-align: center; margin-top: 0; margin-bottom: 0;'><span class='tinyMce-placeholder'>Certificates</span></h1>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</table>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<table class='divider_block block-2' width='100%' border='0' cellpadding='10' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<td class='pad'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<div class='alignment' align='center'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<table border='0' cellpadding='0' cellspacing='0' role='presentation' width='75%' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<td class='divider_inner' style='font-size: 1px; line-height: 1px; border-top: 2px solid #000000;'><span>&#8202;</span></td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</table>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</div>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</table><!--[if mso]><style>#list-r6c1m2 ul{margin: 0 !important; padding: 0 !important;} #list-r6c1m2 ul li{mso-special-format: bullet;}#list-r6c1m2 .levelOne li {margin-top: 0 !important;} #list-r6c1m2 .levelOne {margin-left: -20px !important;}#list-r6c1m2 .levelTwo li {margin-top: 0 !important;} #list-r6c1m2 .levelTwo {margin-left: 10px !important;}#list-r6c1m2 .levelThree li {margin-top: 0 !important;} #list-r6c1m2 .levelThree {margin-left: 40px !important;}</style><![endif]-->\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<table class='list_block block-3' id='list-r6c1m2' width='100%' border='0' cellpadding='10' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; word-break: break-word;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<td class='pad'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<div class='levelOne' style='margin-left: 0;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<ul class='leftList' start='1' style='margin-top: 0; margin-bottom: 0; padding: 0; padding-left: 20px; color: #101112; direction: ltr; font-family: Arial,Helvetica Neue,Helvetica,sans-serif; font-size: 16px; font-weight: 400; letter-spacing: 0; line-height: 120%; text-align: left; mso-line-height-alt: 19.2px; list-style-type: disc;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<li style='margin-bottom: 0; text-align: left;'><a href='https://www.youtube.com/watch?v=0uc3EVM8pHw' target='_blank' style='text-decoration: underline; color: #6133d5;' rel='noopener'>Hızlı Okuma</a></li>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<li style='margin-bottom: 0; text-align: left;'><a href='https://github.com/HuseyinKARADANA/CVSharer' target='_blank' style='text-decoration: underline; color: #6133d5;' rel='noopener'>Github</a></li>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</ul>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</div>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</table>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t</tbody>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t</table>\r\n\t\t\t\t\t\t\t\t\t\t\t\t</td>\r\n\t\t\t\t\t\t\t\t\t\t\t</tr>\r\n\t\t\t\t\t\t\t\t\t\t</tbody>\r\n\t\t\t\t\t\t\t\t\t</table>\r\n\t\t\t\t\t\t\t\t\t<table class='row row-8' align='center' width='100%' border='0' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;'>\r\n\t\t\t\t\t\t\t\t\t\t<tbody>\r\n\t\t\t\t\t\t\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t<td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t<table class='row-content stack' align='center' border='0' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-color: #f4f1eb; color: #000; width: 680px; margin: 0 auto;' width='680'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t<tbody>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<td class='column column-1' width='100%' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; vertical-align: top; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;'>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<div class='spacer_block block-1' style='height:60px;line-height:60px;font-size:1px;'>&#8202;</div>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</td>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</tr>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t</tbody>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t</table>\r\n\t\t\t\t\t\t\t\t\t\t\t\t</td>\r\n\t\t\t\t\t\t\t\t\t\t\t</tr>\r\n\t\t\t\t\t\t\t\t\t\t</tbody>\r\n\t\t\t\t\t\t\t\t\t</table>\r\n\t\t\t\t\t\t\t\t</td>\r\n\t\t\t\t\t\t\t</tr>\r\n\t\t\t\t\t\t</tbody>\r\n\t\t\t\t\t</table><!-- End -->\r\n\t\t\t\t</body>\r\n\r\n\t\t\t\t</html>";
            byte[] pdfBytes = _pdfGenerator.GeneratorPdf(htmlContent);

            return File(pdfBytes, "application/pdf", fileName);
        }
    }
}
