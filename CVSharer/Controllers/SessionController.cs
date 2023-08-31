using BusinessLayer.Abstract;
using EntityLayer.Concrete;
using EntityLayer.DTOs;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using Newtonsoft.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;

namespace CVSharer.Controllers
{
    public class SessionController : Controller
    {
        private readonly IUserService _userService;
        private readonly INotyfService _toast;

        private static Random random = new Random();
        public SessionController(IUserService userService, INotyfService toast)
        {
            _userService = userService;
            _toast = toast;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterDTO registerDTO)
        {
            List<User> users = _userService.GetListAll();

            if(users.Any(u => u.Email == registerDTO.Email))
            {
                _toast.Error("There is a user with this email. Please try another email.");
                return View();
            }

            if(!registerDTO.Password.Equals(registerDTO.PasswordAgain))
            {
                _toast.Error("Passwords do not match!");
                return View();
            }

            if(registerDTO.Password.Length < 8)
            {
                _toast.Error("Your password can be minimum 8 characters!");
                return View();
            }

           
            string message=SendVerifyCode(registerDTO);
            var verifyDto = new VerifyDTO
            {
                registerDTO = registerDTO as RegisterDTO, // RegisterDTO nesnesini doğrudan VerifyDTO içine aktarabilirsiniz
                verifyCode = message.ToString().Trim() // response.Content.ToString() ile elde ettiğiniz değeri "Code" özelliğine atayın
            };
            string serializedModel = JsonConvert.SerializeObject(verifyDto);
            TempData["MyUser"] = serializedModel;
            return RedirectToAction("VerifyAccount", "Session");
            
        }

        [HttpGet]
        public IActionResult Login()
        {
            ClaimsPrincipal claimUser = HttpContext.User;

            if (claimUser.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            var isCredentialsValid = false;
            User validUser = new();

            List<User> userList = _userService.GetListAll();

            foreach (var user in userList)
            {
                try
                {
                    if (BCrypt.Net.BCrypt.Verify(loginDTO.Password, user.Password) && user.Email == loginDTO.Email)
                    {
                        isCredentialsValid = true;
                        validUser = user;
                        break;
                    }
                }
                catch (Exception ex)
                {
                    _toast.Error("An unexpected error is encountered. Please try again later.");
                    return View();
                }
            }

            if (!isCredentialsValid)
            {
                _toast.Error("Email or password are incorrect, please try again.");
                return View();
            }

            if (!validUser.IsActive)
            {
                _toast.Error("This user is not active!");
                return View();
            }

            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, loginDTO.Email),
                new Claim(ClaimTypes.Name, validUser.Name +" "+ validUser.Surname),
                new Claim(ClaimTypes.Role, "User")
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            

            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true,
                IsPersistent = loginDTO.KeepLoggedIn,
            };

            if (loginDTO.KeepLoggedIn)
            {
                properties.ExpiresUtc = DateTime.UtcNow.AddDays(7); // Keep logged in for 7 days
            }
            else
            {
                properties.ExpiresUtc = DateTime.UtcNow.AddHours(2); // Keep logged in for 2 hours
            }

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), properties);

            HttpContext.Response.Cookies.Append("UserId", validUser.UserId.ToString());

            _toast.Success("Login Successfully.");

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> LogOut()
        {
            HttpContext.Response.Cookies.Delete("UserId");
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            _toast.Custom("Logged Out",3,"orange", "ri-logout-box-r-line");
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult VerifyAccount()
        {
            return View();
        }
        [HttpPost]
        public IActionResult VerifyAccount(VerifyDTO verifyDTO)
        {
            var model = JsonConvert.DeserializeObject<VerifyDTO>(TempData["MyUser"] as string);
            if (model.verifyCode == verifyDTO.userCode)
            {
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.registerDTO.Password);
                _userService.Insert(new User()
                {
                    Name = model.registerDTO.Name,
                    Surname = model.registerDTO.Surname,
                    Email = model.registerDTO.Email,
                    Password = hashedPassword,
                    IsActive = true
                });

                _toast.Success("Register Successful.");

                return RedirectToAction("Login");
            }
            else
            {
                _toast.Error("Codes do not match.");
                //TempData["MyUser"] = null;
                return RedirectToAction("Register","Session");
            }
        }

        [HttpPost]
        public string SendVerifyCode(RegisterDTO dto)
        {
            string code = RandomString(8);

            MimeMessage message = new MimeMessage();

            MailboxAddress mailboxAddressFrom = new MailboxAddress("CVSharer", "sharercv@gmail.com");

            message.From.Add(mailboxAddressFrom);

            MailboxAddress mailboxAddressTo = new MailboxAddress(dto.Name, dto.Email);
            message.To.Add(mailboxAddressTo);

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = "<!DOCTYPE html>\r\n\r\n<html lang='en' xmlns:o='urn:schemas-microsoft-com:office:office' xmlns:v='urn:schemas-microsoft-com:vml'>\r\n<head>\r\n<title></title>\r\n" +
                "<meta content='text/html; charset=utf-8' http-equiv='Content-Type'/>\r\n<meta content='width=device-width, initial-scale=1.0' name='viewport'/><!--[if mso]><xml><o:OfficeDocumentSettings>" +
                "<o:PixelsPerInch>96</o:PixelsPerInch><o:AllowPNG/></o:OfficeDocumentSettings></xml><![endif]--><!--[if !mso]><!-->\r\n<link href='https://fonts.googleapis.com/css?family=Merriweather' rel='stylesheet' type='text/css'/>\r\n" +
                "<link href='https://fonts.googleapis.com/css?family=Lato' rel='stylesheet' type='text/css'/><!--<![endif]-->\r\n<style>\r\n\t\t* {\r\n\t\t\tbox-sizing: border-box;\r\n\t\t}\r\n\r\n\t\tbody {\r\n\t\t\tmargin: 0;\r\n\t\t\t" +
                "padding: 0;\r\n\t\t}\r\n\r\n\t\ta[x-apple-data-detectors] {\r\n\t\t\tcolor: inherit !important;\r\n\t\t\ttext-decoration: inherit !important;\r\n\t\t}\r\n\r\n\t\t#MessageViewBody a {\r\n\t\t\tcolor: inherit;\r\n\t\t\t" +
                "text-decoration: none;\r\n\t\t}\r\n\r\n\t\tp {\r\n\t\t\tline-height: inherit\r\n\t\t}\r\n\r\n\t\t.desktop_hide,\r\n\t\t.desktop_hide table {\r\n\t\t\tmso-hide: all;\r\n\t\t\tdisplay: none;\r\n\t\t\tmax-height: 0px;\r\n\t\t\t" +
                "overflow: hidden;\r\n\t\t}\r\n\r\n\t\t.image_block img+div {\r\n\t\t\tdisplay: none;\r\n\t\t}\r\n\r\n\t\t.menu_block.desktop_hide .menu-links span {\r\n\t\t\tmso-hide: all;\r\n\t\t}\r\n\r\n\t\t@media (max-width:660px) {\r\n\r\n\t\t\t" +
                ".desktop_hide table.icons-inner,\r\n\t\t\t.social_block.desktop_hide .social-table {\r\n\t\t\t\tdisplay: inline-block !important;\r\n\t\t\t}\r\n\r\n\t\t\t.icons-inner {\r\n\t\t\t\ttext-align: center;\r\n\t\t\t}\r\n\r\n\t\t\t" +
                ".icons-inner td {\r\n\t\t\t\tmargin: 0 auto;\r\n\t\t\t}\r\n\r\n\t\t\t.menu-checkbox[type=checkbox]~.menu-links {\r\n\t\t\t\tdisplay: none !important;\r\n\t\t\t\tpadding: 5px 0;\r\n\t\t\t}\r\n\r\n\t\t\t" +
                ".menu-checkbox[type=checkbox]:checked~.menu-trigger .menu-open {\r\n\t\t\t\tdisplay: none !important;\r\n\t\t\t}\r\n\r\n\t\t\t.menu-checkbox[type=checkbox]:checked~.menu-links,\r\n\t\t\t.menu-checkbox[type=checkbox]~.menu-trigger {\r\n\t\t\t\t" +
                "display: block !important;\r\n\t\t\t\tmax-width: none !important;\r\n\t\t\t\tmax-height: none !important;\r\n\t\t\t\tfont-size: inherit !important;\r\n\t\t\t}\r\n\r\n\t\t\t.menu-checkbox[type=checkbox]~.menu-links>a,\r\n\t\t\t" +
                ".menu-checkbox[type=checkbox]~.menu-links>span.label {\r\n\t\t\t\tdisplay: block !important;\r\n\t\t\t\ttext-align: center;\r\n\t\t\t}\r\n\r\n\t\t\t.menu-checkbox[type=checkbox]:checked~.menu-trigger .menu-close {\r\n\t\t\t\t" +
                "display: block !important;\r\n\t\t\t}\r\n\r\n\t\t\t.mobile_hide {\r\n\t\t\t\tdisplay: none;\r\n\t\t\t}\r\n\r\n\t\t\t.row-content {\r\n\t\t\t\twidth: 100% !important;\r\n\t\t\t}\r\n\r\n\t\t\t.stack .column {\r\n\t\t\t\twidth: 100%;\r\n\t\t\t\t" +
                "display: block;\r\n\t\t\t}\r\n\r\n\t\t\t.mobile_hide {\r\n\t\t\t\tmin-height: 0;\r\n\t\t\t\tmax-height: 0;\r\n\t\t\t\tmax-width: 0;\r\n\t\t\t\toverflow: hidden;\r\n\t\t\t\tfont-size: 0px;\r\n\t\t\t}\r\n\r\n\t\t\t.desktop_hide,\r\n\t\t\t" +
                ".desktop_hide table {\r\n\t\t\t\tdisplay: table !important;\r\n\t\t\t\tmax-height: none !important;\r\n\t\t\t}\r\n\t\t}\r\n\r\n\t\t#memu-r0c1m1:checked~.menu-links {\r\n\t\t\tbackground-color: #f6d16c !important;\r\n\t\t}\r\n\r\n\t\t" +
                "#memu-r0c1m1:checked~.menu-links a,\r\n\t\t#memu-r0c1m1:checked~.menu-links span {\r\n\t\t\tcolor: #fff !important;\r\n\t\t}\r\n\t</style>\r\n</head>\r\n<body style='margin: 0; background-color: #fff; padding: 0; -webkit-text-size-adjust: none; text-size-adjust: none;'>\r\n" +
                "<table border='0' cellpadding='0' cellspacing='0' class='nl-container' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-color: #fff;' width='100%'>\r\n<tbody>\r\n<tr>\r\n<td>\r\n" +
                "<table align='center' border='0' cellpadding='0' cellspacing='0' class='row row-1' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-color: #f0dbb3;' width='100%'>\r\n<tbody>\r\n<tr>\r\n<td>\r\n" +
                "<table align='center' border='0' cellpadding='0' cellspacing='0' class='row-content' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; color: #000; width: 640px; margin: 0 auto;' width='640'>\r\n<tbody>\r\n" +
                "<tr>\r\n<td class='column column-1' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; padding-bottom: 5px; padding-top: 5px; vertical-align: top; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;' width='50%'>\r\n" +
                "<table border='0' cellpadding='0' cellspacing='0' class='image_block block-1' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;' width='100%'>\r\n<tr>\r\n<td class='pad' style='padding-left:20px;padding-top:20px;width:100%;padding-right:0px;'>\r\n" +
                "<div align='left' class='alignment' style='line-height:10px'><img alt='Image' src='https://i.hizliresim.com/amapydk.png' style='display: block; height: auto; border: 0; max-width: 64px; width: 100%;' title='Image' width='64'/></div>\r\n</td>\r\n</tr>\r\n</table>\r\n</td>\r\n" +
                "<td class='column column-2' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; padding-bottom: 5px; padding-top: 5px; vertical-align: top; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;' width='50%'>\r\n" +
                "<div class='spacer_block block-1 mobile_hide' style='height:20px;line-height:20px;font-size:1px;'> </div>\r\n<table border='0' cellpadding='0' cellspacing='0' class='menu_block block-2' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;' width='100%'>\r\n" +
                "<tr>\r\n<td class='pad' style='color:#000000;font-family:inherit;font-size:18px;padding-top:15px;text-align:center;'>\r\n<table border='0' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;' width='100%'>\r\n<tr>\r\n" +
                "<td class='alignment' style='text-align:center;font-size:0px;'><!--[if !mso]><!--><input class='menu-checkbox' id='memu-r0c1m1' style='display:none !important;max-height:0;visibility:hidden;' type='checkbox'/><!--<![endif]-->\r\n" +
                "<div class='menu-trigger' style='display:none;max-height:0px;max-width:0px;font-size:0px;overflow:hidden;'><label class='menu-label' for='memu-r0c1m1' style='height: 48px; width: 48px; display: inline-block; cursor: pointer; mso-hide: all; user-select: none; align: center; text-align: center; color: #ffffff; text-decoration: none; background-color: #f6d16c; border-radius: 0;'>" +
                "<span class='menu-open' style='mso-hide:all;font-size:38px;line-height:42px;'>☰</span><span class='menu-close' style='display:none;mso-hide:all;font-size:38px;line-height:48px;'>✕</span></label></div>\r\n<div class='menu-links'><!--[if mso]><table role='presentation' border='0' cellpadding='0' cellspacing='0' align='center' style=''><tr style='text-align:center;'>" +
                "<![endif]--><!--[if mso]><td style='padding-top:5px;padding-right:5px;padding-bottom:5px;padding-left:5px'><![endif]--><a href='https://cvsharer.com' style='mso-hide:false;padding-top:5px;padding-bottom:5px;padding-left:5px;padding-right:5px;display:inline-block;color:#0068A5;font-family:Merriwheater, Georgia, serif;font-size:18px;text-decoration:none;letter-spacing:normal;' target='_self'>cvsharer.com</a>" +
                "<!--[if mso]></td><![endif]--><!--[if mso]></tr></table><![endif]--></div>\r\n</td>\r\n</tr>\r\n</table>\r\n</td>\r\n</tr>\r\n</table>\r\n</td>\r\n</tr>\r\n</tbody>\r\n</table>\r\n</td>\r\n</tr>\r\n</tbody>\r\n</table>\r\n<table align='center' border='0' cellpadding='0' cellspacing='0' class='row row-2' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-color: #f0dbb3; background-image: url(https://i.hizliresim.com/kuknsdk.png); background-position: top center; background-repeat: no-repeat;' width='100%'>\r\n<tbody>\r\n<tr>\r\n<td>\r\n<table align='center' border='0' cellpadding='0' cellspacing='0' class='row-content stack' role='presentation'" +
                " style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; color: #000; width: 640px; margin: 0 auto;' width='640'>\r\n<tbody>\r\n<tr>\r\n<td class='column column-1' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; vertical-align: top; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;' width='100%'>\r\n<table border='0' cellpadding='10' cellspacing='0' class='divider_block block-1' " +
                "role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;' width='100%'>\r\n<tr>\r\n<td class='pad'>\r\n<div align='center' class='alignment'>\r\n<table border='0' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;' width='100%'>\r\n<tr>\r\n" +
                "<td class='divider_inner' style='font-size: 1px; line-height: 1px; border-top: 1px solid #000000;'><span> </span></td>\r\n</tr>\r\n</table>\r\n</div>\r\n</td>\r\n</tr>\r\n</table>\r\n<table border='0' cellpadding='0' cellspacing='0' class='paragraph_block block-2' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; word-break: break-word;' width='100%'>\r\n<tr>\r\n<td class='pad' style='padding-bottom:60px;padding-left:40px;padding-right:40px;padding-top:40px;'>\r\n" +
                "<div style='color:#ffffff;font-family:Merriwheater, Georgia, serif;font-size:30px;line-height:150%;text-align:center;mso-line-height-alt:45px;'>\r\n<p style='margin: 0; word-break: break-word;'><span style='color: #000000;'><strong>CVSharer Verification</strong></span></p>\r\n</div>\r\n</td>\r\n</tr>\r\n</table>\r\n</td>\r\n</tr>\r\n</tbody>\r\n</table>\r\n</td>\r\n</tr>\r\n</tbody>\r\n</table>\r\n<table align='center' border='0' cellpadding='0' cellspacing='0' " +
                "class='row row-3' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-color: #ffffff;' width='100%'>\r\n<tbody>\r\n<tr>\r\n<td>\r\n<table align='center' border='0' cellpadding='0' cellspacing='0' class='row-content stack' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; color: #000; width: 640px; margin: 0 auto;' width='640'>\r\n<tbody>\r\n<tr>\r\n<td class='column column-1' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; vertical-align: top; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;' width='100%'>\r\n" +
                "<div class='spacer_block block-1' style='height:15px;line-height:15px;font-size:1px;'> </div>\r\n</td>\r\n</tr>\r\n</tbody>\r\n</table>\r\n</td>\r\n</tr>\r\n</tbody>\r\n</table>\r\n<table align='center' border='0' cellpadding='0' cellspacing='0' class='row row-4' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;' width='100%'>\r\n<tbody>\r\n<tr>\r\n<td>\r\n" +
                "<table align='center' border='0' cellpadding='0' cellspacing='0' class='row-content stack' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-color: #fff; color: #000; width: 640px; margin: 0 auto;' width='640'>\r\n<tbody>\r\n<tr>\r\n<td class='column column-1' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; vertical-align: top; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;' width='100%'>\r\n<table border='0' cellpadding='0' cellspacing='0' class='paragraph_block block-1' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; word-break: break-word;' width='100%'>\r\n" +
                "<tr>\r\n<td class='pad' style='padding-left:15px;padding-right:10px;padding-top:23px;'>\r\n" +
                "<div style='color:#000000;font-family:Merriwheater, Georgia, serif;font-size:18px;line-height:150%;text-align:left;mso-line-height-alt:27px;'>\r\n<p style='margin: 0; word-break: break-word;'><span style='color: #f6d16c;'><strong><span>DESCRIPTION</span></strong></span></p>\r\n</div>\r\n</td>\r\n</tr>\r\n</table>\r\n<table border='0' cellpadding='0' cellspacing='0' class='paragraph_block block-2' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; word-break: break-word;' width='100%'>\r\n<tr>\r\n<td class='pad' style='padding-bottom:10px;padding-left:15px;padding-right:15px;'>\r\n<div style='color:#555555;font-family:'Merriwheater','Georgia',serif;font-size:15px;" +
                "line-height:150%;text-align:left;mso-line-height-alt:22.5px;'>\r\n<p style='margin: 0; word-break: break-word;'>You've reached the final step of your career journey! Your verification code is ready to explore the opportunities CVSharer offers and shape your professional future. To complete your account and access all features, find your verification code below:</p>\r\n</div>\r\n</td>\r\n</tr>\r\n</table>\r\n<table border='0'" +
                " cellpadding='0' cellspacing='0' class='divider_block block-3' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;' width='100%'>\r\n<tr>\r\n<td class='pad' style='padding-bottom:10px;padding-left:15px;padding-right:15px;padding-top:10px;'>\r\n<div align='center' class='alignment'>\r\n" +
                "<table border='0' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;' width='100%'>\r\n<tr>\r\n<td class='divider_inner' style='font-size: 1px; line-height: 1px; border-top: 1px solid #E9EBEB;'><span> </span></td>\r\n</tr>\r\n</table>\r\n</div>\r\n</td>\r\n</tr>\r\n</table>\r\n<table border='0' cellpadding='0' cellspacing='0' class='paragraph_block block-4' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; word-break: break-word;' width='100%'>\r\n" +
                "<tr>\r\n<td class='pad' style='padding-left:15px;padding-right:10px;padding-top:23px;'>\r\n<div style='color:#000000;font-family:Merriwheater, Georgia, serif;font-size:18px;line-height:150%;text-align:left;mso-line-height-alt:27px;'>\r\n<p style='margin: 0; word-break: break-word;'><span style='color: #f6d16c;'><strong><span>Verify Code</span></strong></span></p>\r\n</div>\r\n</td>\r\n</tr>\r\n</table>\r\n<table border='0' cellpadding='0' " +
                "cellspacing='0' class='icons_block block-5' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;' width='100%'>\r\n<tr>\r\n<td class='pad' style='vertical-align: middle; color: #000000; font-family: inherit; font-size: 18px; padding-left: 15px; padding-top: 15px; text-align: left;'>\r\n<table cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;' width='100%'>\r\n<tr>\r\n<td class='alignment' style='vertical-align: middle; text-align: left;'><!--[if vml]><table align='left' cellpadding='0' cellspacing='0' role='presentation' style='display:inline-block;" +
                "padding-left:0px;padding-right:0px;mso-table-lspace: 0pt;mso-table-rspace: 0pt;'><![endif]-->\r\n<!--[if !vml]><!-->\r\n<table cellpadding='0' cellspacing='0' class='icons-inner' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; display: inline-block; margin-right: -4px; padding-left: 0px; padding-right: 0px;'><!--<![endif]-->\r\n<tr>\r\n<td style='vertical-align: middle; text-align: center; padding-top: 5px; padding-bottom: 5px; padding-left: 5px; padding-right: 10px;'>" +
                "<img align='center' alt='Default' class='icon' height='32' src='https://i.hizliresim.com/9b099ky.png' style='display: block; height: auto; margin: 0 auto; border: 0;' width='32'/></td>\r\n<td style='font-family: Merriwheater, Georgia, serif; font-size: 18px; color: #000000; vertical-align: middle; letter-spacing: undefined; text-align: left;'>"+code+"</td>\r\n</tr>\r\n</table>\r\n</td>\r\n</tr>\r\n</table>\r\n</td>\r\n</tr>\r\n</table>\r\n</td>\r\n</tr>\r\n</tbody>\r\n</table>\r\n</td>\r\n</tr>\r\n</tbody>\r\n</table>\r\n<table align='center' border='0' cellpadding='0' cellspacing='0' class='row row-5' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;' width='100%'>\r\n" +
                "<tbody>\r\n<tr>\r\n<td>\r\n<table align='center' border='0' cellpadding='0' cellspacing='0' class='row-content stack' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; color: #000; width: 640px; margin: 0 auto;' width='640'>\r\n<tbody>\r\n<tr>\r\n<td class='column column-1' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; padding-bottom: 5px; padding-top: 5px; vertical-align: top; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;' width='100%'>\r\n<div class='spacer_block block-1' style='height:20px;line-height:20px;font-size:1px;'> </div>\r\n</td>\r\n</tr>\r\n</tbody>\r\n</table>\r\n</td>\r\n" +
                "</tr>\r\n</tbody>\r\n</table>\r\n<table align='center' border='0' cellpadding='0' cellspacing='0' class='row row-6' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;' width='100%'>\r\n<tbody>\r\n<tr>\r\n<td>\r\n<table align='center' border='0' cellpadding='0' cellspacing='0' class='row-content stack' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; color: #000; width: 640px; margin: 0 auto;' width='640'>\r\n" +
                "<tbody>\r\n<tr>\r\n<td class='column column-1' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; vertical-align: top; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;' width='100%'>\r\n<div class='spacer_block block-1' style='height:50px;line-height:50px;" +
                "font-size:1px;'> </div>\r\n</td>\r\n</tr>\r\n</tbody>\r\n</table>\r\n</td>\r\n</tr>\r\n</tbody>\r\n</table>\r\n<table align='center' border='0' cellpadding='0' cellspacing='0' class='row row-7' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-color: #f0dbb3;' width='100%'>\r\n<tbody>\r\n<tr>\r\n<td>\r\n<table align='center' border='0' cellpadding='0' cellspacing='0' class='row-content stack' role='presentation' style='mso-table-lspace: 0pt; " +
                "mso-table-rspace: 0pt; color: #000; width: 640px; margin: 0 auto;' width='640'>\r\n<tbody>\r\n<tr>\r\n<td class='column column-1' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; vertical-align: top; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;' width='100%'>\r\n<div class='spacer_block block-1' style='height:5px;line-height:5px;font-size:1px;'> </div>\r\n</td>\r\n</tr>\r\n</tbody>\r\n" +
                "</table>\r\n</td>\r\n</tr>\r\n</tbody>\r\n</table>\r\n<table align='center' border='0' cellpadding='0' cellspacing='0' class='row row-8' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-color: #f0dbb3;' width='100%'>\r\n<tbody>\r\n<tr>\r\n<td>\r\n<table align='center' border='0' cellpadding='0' cellspacing='0' class='row-content stack' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; color: #000; width: 640px; margin: 0 auto;' width='640'>\r\n<tbody>\r\n<tr>\r\n<td class='column column-1' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; " +
                "padding-bottom: 5px; padding-top: 5px; vertical-align: top; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;' width='33.333333333333336%'>\r\n<table border='0' cellpadding='5' cellspacing='0' class='image_block block-1' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;' width='100%'>\r\n<tr>\r\n<td class='pad'>\r\n<div align='center' class='alignment' style='line-height:10px'><img alt='Alternate text' src='https://i.hizliresim.com/amapydk.png' style='display: block; height: auto; border: 0; max-width: 85px; width: 100%;' title='Alternate text' width='85'/></div>\r\n</td>\r\n</tr>\r\n</table>\r\n<table border='0' cellpadding='5' " +
                "cellspacing='0' class='paragraph_block block-2' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; word-break: break-word;' width='100%'>\r\n<tr>\r\n<td class='pad'>\r\n<div style='color:#000000;font-family:Merriwheater, Georgia, serif;font-size:14px;line-height:120%;text-align:center;mso-line-height-alt:16.8px;'>\r\n<p style='margin: 0;'>CVSharer.com</p>\r\n</div>\r\n</td>\r\n</tr>\r\n</table>\r\n" +
                "<table border='0' cellpadding='0' cellspacing='0' class='button_block block-3' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;' width='100%'>\r\n<tr>\r\n<td class='pad' style='padding-left:40px;padding-right:40px;padding-top:10px;text-align:center;'>\r\n<div align='center' class='alignment'><!--[if mso]><v:roundrect xmlns:v='urn:schemas-microsoft-com:vml' xmlns:w='urn:schemas-microsoft-com:office:word' href='https://cvsharer.com/' style='height:29px;width:118px;v-text-anchor:middle;' arcsize='118%' strokeweight='0.75pt' strokecolor='#F6D16C' fillcolor='#ffffff'><w:anchorlock/><v:textbox inset='0px,0px,0px,0px'><center style='color:#000000; " +
                "font-family:Georgia, serif; font-size:14px'><![endif]--><a href='https://cvsharer.com/' style='text-decoration:none;display:inline-block;color:#000000;background-color:#ffffff;border-radius:34px;width:auto;border-top:1px solid #F6D16C;font-weight:undefined;border-right:1px solid #F6D16C;border-bottom:1px solid #F6D16C;border-left:1px solid #F6D16C;padding-top:0px;padding-bottom:0px;font-family:Merriwheater, Georgia, serif;font-size:14px;text-align:center;" +
                "mso-border-alt:none;word-break:keep-all;' target='_blank'><span style='padding-left:18px;padding-right:18px;font-size:14px;display:inline-block;letter-spacing:normal;'><span style='margin:0;word-break:break-word;'><span data-mce-style='' style='line-height: 28px;'><strong>Visit Site</strong></span>" +
                "</span></span></a><!--[if mso]></center></v:textbox></v:roundrect><![endif]--></div>\r\n</td>\r\n</tr>\r\n</table>\r\n<div class='spacer_block block-4' style='height:30px;line-height:30px;font-size:1px;'> </div>\r\n</td>\r\n<td class='column column-2' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; padding-bottom: 5px; padding-top: 5px; vertical-align: top; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;' width='33.333333333333336%'>\r\n" +
                "<table border='0' cellpadding='0' cellspacing='0' class='image_block block-1' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;' width='100%'>\r\n<tr>\r\n<td class='pad' style='padding-top:15px;width:100%;padding-right:0px;padding-left:0px;'>\r\n<div align='center' class='alignment' style='line-height:10px'><img alt='Alternate text' src='https://i.hizliresim.com/ow5obwd.png' style='display: block; height: auto; border: 0; max-width: 75px; " +
                "width: 100%;' title='Alternate text' width='75'/></div>\r\n</td>\r\n</tr>\r\n</table>\r\n<table border='0' cellpadding='10' cellspacing='0' class='paragraph_block block-2' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; word-break: break-word;' width='100%'>\r\n<tr>\r\n<td class='pad'>\r\n<div style='color:#000000;font-family:Merriwheater, Georgia, serif;font-size:14px;line-height:120%;text-align:center;mso-line-height-alt:16.8px;'>\r\n<p style='margin: 0; word-break: break-word;'>Terms and Conditions</p>\r\n</div>\r\n</td>\r\n</tr>\r\n</table>\r\n<table border='0' cellpadding='0' cellspacing='0' " +
                "class='button_block block-3' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;' width='100%'>\r\n<tr>\r\n<td class='pad' style='padding-left:40px;padding-right:40px;padding-top:10px;text-align:center;'>\r\n<div align='center' class='alignment'><!--[if mso]><v:roundrect xmlns:v='urn:schemas-microsoft-com:vml' xmlns:w='urn:schemas-microsoft-com:office:word' style='height:29px;width:118px;v-text-anchor:middle;' arcsize='118%' strokeweight='0.75pt' strokecolor='#F6D16C' fillcolor='#ffffff'>" +
                "<w:anchorlock/><v:textbox inset='0px,0px,0px,0px'><center style='color:#000000; font-family:Georgia, serif; font-size:14px'><![endif]-->\r\n<div style='text-decoration:none;display:inline-block;color:#000000;background-color:#ffffff;border-radius:34px;width:auto;border-top:1px solid #F6D16C;font-weight:undefined;border-right:1px solid #F6D16C;border-bottom:1px solid #F6D16C;border-left:1px solid #F6D16C;padding-top:0px;padding-bottom:0px;font-family:Merriwheater, Georgia, serif;font-size:14px;text-align:center;mso-border-alt:none;word-break:keep-all;'><span style='padding-left:18px;padding-right:18px;font-size:14px;display:inline-block;letter-spacing:normal;'>" +
                "<span style='margin:0;word-break:break-word;'><span data-mce-style='' style='line-height: 28px;'><strong>Read More</strong></span></span></span></div><!--[if mso]></center></v:textbox></v:roundrect><![endif]-->\r\n</div>\r\n</td>\r\n</tr>\r\n</table>\r\n<div class='spacer_block block-4' style='height:30px;line-height:30px;font-size:1px;'> </div>\r\n</td>\r\n<td class='column column-3' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; padding-bottom: 5px; padding-top: 5px; vertical-align: top; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;' width='33.333333333333336%'>\r\n<table border='0' cellpadding='0' " +
                "cellspacing='0' class='image_block block-1' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;' width='100%'>\r\n<tr>\r\n<td class='pad' style='padding-top:25px;width:100%;padding-right:0px;padding-left:0px;'>\r\n<div align='center' class='alignment' style='line-height:10px'><img alt='Alternate text' src='https://i.hizliresim.com/gnbzcwt.png' style='display: block; height: auto; border: 0; max-width: 64px; width: 100%;' title='Alternate text' width='64'/>" +
                "</div>\r\n</td>\r\n</tr>\r\n</table>\r\n<table border='0' cellpadding='10' cellspacing='0' class='paragraph_block block-2' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; word-break: break-word;' width='100%'>\r\n<tr>\r\n<td class='pad'>\r\n" +
                "<div style='color:#000000;font-family:Merriwheater, Georgia, serif;font-size:14px;line-height:120%;text-align:center;mso-line-height-alt:16.8px;'>\r\n<p style='margin: 0;'>Privacy Policy</p>\r\n</div>\r\n</td>\r\n</tr>\r\n</table>\r\n<table border='0' cellpadding='0' cellspacing='0' class='button_block block-3' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;' width='100%'>\r\n<tr>\r\n<td class='pad' style='padding-left:40px;padding-right:40px;padding-top:10px;text-align:center;'>\r\n" +
                "<div align='center' class='alignment'><!--[if mso]><v:roundrect xmlns:v='urn:schemas-microsoft-com:vml' xmlns:w='urn:schemas-microsoft-com:office:word' style='height:29px;width:118px;v-text-anchor:middle;' arcsize='118%' strokeweight='0.75pt' strokecolor='#F6D16C' fillcolor='#ffffff'><w:anchorlock/><v:textbox inset='0px,0px,0px,0px'><center style='color:#000000; font-family:Georgia, serif; font-size:14px'><![endif]-->\r\n<div style='text-decoration:none;" +
                "display:inline-block;color:#000000;background-color:#ffffff;border-radius:34px;width:auto;border-top:1px solid #F6D16C;font-weight:undefined;border-right:1px solid #F6D16C;border-bottom:1px solid #F6D16C;border-left:1px solid #F6D16C;padding-top:0px;padding-bottom:0px;font-family:Merriwheater, Georgia, serif;font-size:14px;text-align:center;mso-border-alt:none;word-break:keep-all;'><span style='padding-left:18px;padding-right:18px;font-size:14px;display:inline-block;letter-spacing:normal;'><span style='margin:0;word-break:break-word;'><span data-mce-style='' style='line-height: 28px;'><strong>Read More</strong></span></span>" +
                "</span></div><!--[if mso]></center></v:textbox></v:roundrect><![endif]-->\r\n</div>\r\n</td>\r\n</tr>\r\n</table>\r\n<div class='spacer_block block-4' style='height:30px;line-height:30px;font-size:1px;'> </div>\r\n</td>\r\n</tr>\r\n</tbody>\r\n</table>\r\n</td>\r\n</tr>\r\n</tbody>\r\n</table>\r\n<table align='center' border='0' cellpadding='0' cellspacing='0' class='row row-9' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-color: #000000;' width='100%'>\r\n<tbody>\r\n<tr>\r\n<td>\r\n" +
                "<table align='center' border='0' cellpadding='0' cellspacing='0' class='row-content stack' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; color: #000; width: 640px; margin: 0 auto;' width='640'>\r\n<tbody>\r\n<tr>\r\n<td class='column column-1' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; padding-bottom: 5px; padding-top: 5px; vertical-align: top; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;' width='75%'>\r\n<table border='0' cellpadding='0' cellspacing='0' class='paragraph_block block-1' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; word-break: break-word;' width='100%'>\r\n" +
                "<tr>\r\n<td class='pad' style='padding-bottom:10px;padding-left:20px;padding-right:20px;padding-top:10px;'>\r\n<div style='color:#555555;font-family:'Merriwheater','Georgia',serif;font-size:18px;line-height:120%;text-align:left;mso-line-height-alt:21.599999999999998px;'>\r\n<p style='margin: 0; word-break: break-word;'><em><span style='color:#f6d16c;'><span>Contact us:</span></span></em></p>\r\n</div>\r\n</td>\r\n</tr>\r\n</table>\r\n<table border='0' cellpadding='0' cellspacing='0' class='paragraph_block block-2' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; word-break: break-word;' width='100%'>\r\n<tr>\r\n<td class='pad' style='padding-bottom:10px;" +
                "padding-left:20px;padding-right:20px;'>\r\n<div style='color:#ffffff;font-family:'Lato',Tahoma,Verdana,Segoe,sans-serif;font-size:14px;line-height:120%;text-align:left;mso-line-height-alt:16.8px;'>\r\n<p style='margin: 0; word-break: break-word;'>sharercv@gmail.com</p>\r\n</div>\r\n</td>\r\n</tr>\r\n</table>\r\n<table border='0' cellpadding='0' cellspacing='0' class='paragraph_block block-3' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; " +
                "word-break: break-word;' width='100%'>\r\n<tr>\r\n<td class='pad' style='padding-bottom:10px;padding-left:20px;padding-right:20px;padding-top:10px;'>\r\n<div style='color:#ffffff;font-family:'Lato',Tahoma,Verdana,Segoe,sans-serif;font-size:13px;line-height:120%;text-align:left;" +
                "mso-line-height-alt:15.6px;'>\r\n<p style='margin: 0; word-break: break-word;color:#ffffff'>CVSharer is a CV creation platform that helps you steer your career effectively. With its user-friendly interface, customizable templates, and easy sharing features, it enhances the efficiency of your job search process.</p>\r\n</div>\r\n</td>\r\n</tr>\r\n</table>\r\n</td>\r\n<td class='column column-2' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; padding-bottom: 5px; padding-top: 5px;" +
                " vertical-align: top; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;' width='25%'>\r\n<table border='0' cellpadding='0' cellspacing='0' class='image_block block-1' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;' width='100%'>\r\n<tr>\r\n<td class='pad' style='padding-bottom:10px;padding-left:10px;padding-right:20px;padding-top:15px;width:100%;'>\r\n<div align='left' class='alignment' style='line-height:10px'>" +
                "<img alt='Image' src='https://i.hizliresim.com/amapydk.png' style='display: block; height: auto; border: 0; max-width: 60.8px; width: 100%;' title='Image' width='60.8'/></div>\r\n</td>\r\n</tr>\r\n</table>\r\n<table border='0' cellpadding='10' cellspacing='0' class='social_block block-2' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;' width='100%'>\r\n<tr>\r\n<td class='pad'>\r\n<div align='left' class='alignment'>\r\n<table border='0' cellpadding='0' cellspacing='0' class='social-table' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; display: inline-block;' width='138px'>\r\n" +
                "<tr>\r\n<td style='padding:0 14px 0 0;'><a href='https://www.facebook.com' target='_blank'><img alt='Facebook' height='32' src='https://i.hizliresim.com/xxorz5f.png' style='display: block; height: auto; border: 0;' title='Facebook' width='32'/></a></td>\r\n<td style='padding:0 14px 0 0;'><a href='https://www.instagram.com' target='_blank'><img alt='Instagram' height='32' src='https://i.hizliresim.com/ohqt09d.png' style='display: block; height: auto; border: 0;' title='Instagram' width='32'/></a></td>\r\n" +
                "<td style='padding:0 14px 0 0;'><a href='https://www.linkedin.com' target='_blank'><img alt='LinkedIn' height='32' src='https://i.hizliresim.com/olo720c.png' style='display: block; height: auto; border: 0;' title='LinkedIn' width='32'/></a></td>\r\n</tr>\r\n</table>\r\n</div>\r\n</td>\r\n</tr>\r\n</table>\r\n</td>\r\n</tr>\r\n</tbody>\r\n</table>\r\n</td>\r\n</tr>\r\n</tbody>\r\n</table>\r\n<table align='center' border='0' cellpadding='0' cellspacing='0' class='row row-10' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;' width='100%'>\r\n<tbody>\r\n<tr>\r\n<td>\r\n" +
                "<table align='center' border='0' cellpadding='0' cellspacing='0' class='row-content stack' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; color: #000; width: 640px; margin: 0 auto;' width='640'>\r\n<tbody>\r\n<tr>\r\n<td class='column column-1' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; padding-bottom: 5px; padding-top: 5px; vertical-align: top; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;' width='100%'>\r\n<table border='0' cellpadding='0' cellspacing='0' class='icons_block block-1' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;' width='100%'>\r\n\r\n" +
                "</table>\r\n</td>\r\n</tr>\r\n</tbody>\r\n</table>\r\n</td>\r\n</tr>\r\n</tbody>\r\n</table>\r\n</td>\r\n</tr>\r\n</tbody>\r\n</table><!-- End -->\r\n</body>\r\n</html>";
            message.Body = bodyBuilder.ToMessageBody();
            message.Subject = "Register Code";


            SmtpClient client = new SmtpClient();
            client.Connect("smtp.gmail.com", 587, false);
            client.Authenticate("sharercv@gmail.com", "dhyllxfxqrnqpdai");
            client.Send(message);
            client.Disconnect(true);
            return code;

        }
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
