using G3.Dtos;
using G3.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Policy;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace G3.Controllers
{
    [Route("/auth")]
    public class AuthController : Controller
    {
        private readonly SWPContext _context;
        private readonly string clientId;
        private readonly string clientSecret;
        private readonly string redirectUri;

        private readonly IConfiguration Configuration;

        public AuthController(SWPContext context, IConfiguration configuration)
        {
            Configuration = configuration;
            clientId = Configuration["Authentication:Google:ClientId"];
            clientSecret = Configuration["Authentication:Google:ClientSecret"];
            redirectUri = Configuration["Authentication:Google:RedirectUri"];

            _context = context;
        }

        [HttpPost]
        public ActionResult Google()
        {
            var state = Guid.NewGuid().ToString();
            var authorizeUrl = string.Format("https://accounts.google.com/o/oauth2/auth?client_id={0}&redirect_uri={1}&response_type=code&scope=email profile&state={2}", clientId, redirectUri, state);
            return Redirect(authorizeUrl);
        }

        [Route("google/callback")]
        public async Task<ActionResult> GoogleCallback([FromQuery] string code, [FromServices] IMailService mailService)
        {

            string authorizationCode = code;

            var httpClient = new HttpClient();
            var tokenRequest = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "code", authorizationCode },
                { "client_id", clientId },
                { "client_secret", clientSecret },
                { "redirect_uri", redirectUri },
                { "grant_type", "authorization_code" }
            });
            var tokenResponse = await httpClient.PostAsync("https://accounts.google.com/o/oauth2/token", tokenRequest);
            var tokenContent = await tokenResponse.Content.ReadAsStringAsync();
            GoogloInfo tokenInfo = JsonSerializer.Deserialize<GoogloInfo>(tokenContent)!;

            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenInfo.access_token);
            var peopleResponse = await httpClient.GetAsync("https://www.googleapis.com/oauth2/v2/userinfo");
            string mailContent = await peopleResponse.Content.ReadAsStringAsync();
            MailInfo mailInfo = JsonSerializer.Deserialize<MailInfo>(mailContent)!;
            string email = mailInfo.email;

            string? domain = mailService.GetDomain(email);
            if (domain == null)
            {
                ViewBag.AlertMessage = "Email Domain False";
                return View();
            }

            Setting? domainSetting = _context.Settings.FirstOrDefault(setting => setting.Type == "DOMAIN" && setting.Value == domain);
            if (domainSetting == null)
            {
                ViewBag.AlertMessage = "Email Domain Not Accept";
                return View();
            }

            User? user = _context.Users.Include(s => s.RoleSetting).FirstOrDefault(user => user.Email == email);

            if (user != null && user.Status == false)
            {
                ViewBag.AlertMessage = "User Blocked";
                return RedirectToAction(nameof(SignIn), "Auth");
            }

            Setting? roleSetting = _context.Settings.FirstOrDefault(setting => setting.Type == "ROLE" && setting.Value == "STUDENT");
            if (roleSetting == null)
            {
                ViewBag.AlertMessage = "Student Role Not Found";
                return View();
            }

            if (user == null)
            {
                user = new()
                {
                    Email = email,
                    DomainSettingId = domainSetting.SettingId,
                    RoleSettingId = roleSetting.SettingId,
                    Name = mailService.GetAddress(email)!,
                    Status = true
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

            }

            string userJsonString = JsonSerializer.Serialize(user!, new JsonSerializerOptions()
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            });
            HttpContext.Session.SetString("User", userJsonString);

            switch (user.RoleSetting.Value)
            {
                case "ADMIN":
                    return View("Views/Admin/AdminHome.cshtml");
                case "SUBJECT_MANAGER":
                    return View("Views/Manager/ManagerHome.cshtml");
                case "CLASS_MANAGER":
                    return View("Views/ClassManager/ClassManager.cshtml");
                /*case "MENTOR":
                    return View("Views/Admin/AdminHome.cshtml");
                case "STUDENT":
                    return View("Views/Admin/AdminHome.cshtml");*/
            }

            return View();
        }

        [Route("sign-up")]
        public IActionResult SignUp()
        {
            return View(new SignUpDto());
        }

        [Route("confirm/{token}")]
        public async Task<IActionResult> Confirm(string token)
        {
            User? user = _context.Users.FirstOrDefault(user => user.ConfirmToken == token);

            if (user == null)
            {
                ViewBag.AlertMessage = "Token invalid";
                return View();
            }

            user.ConfirmToken = null;
            user.Status = true;
            user.ConfirmTokenVerifyAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(SignIn));
        }

        [Route("sign-in")]
        public IActionResult SignIn()
        {
            return View(new SignInDto());
        }

        [Route("change-password")]
        [AuthActionFilter]
        public IActionResult ChangePassword()
        {
            return View(new ChangePasswordDto());
        }

        [Route("forgot-password")]
        public IActionResult ForgotPassword()
        {
            return View(new ForgotPasswordDto());
        }

        [Route("reset-password/{token}")]
        public IActionResult ResetPassword()
        {
            return View(new ResetPasswordDto());
        }

        [Route("sign-up")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(SignUpDto signUpDto, [FromServices] IMailService mailService, [FromServices] IHashService hashService)
        {
            if (!ModelState.IsValid) return View();
            if (signUpDto.Password != signUpDto.ConfirmPassword)
            {
                ViewBag.AlertMessage = "Password not match";
                return View(signUpDto);
            }

            string? domain = mailService.GetDomain(signUpDto.Email);
            if (domain == null)
            {
                ViewBag.AlertMessage = "Email Domain False";
                return View(signUpDto);
            }

            Setting? domainSetting = _context.Settings.FirstOrDefault(setting => setting.Type == "DOMAIN" && setting.Value == domain);
            if (domainSetting == null)
            {
                ViewBag.AlertMessage = "Email Domain Not Accept";
                return View(signUpDto);
            }

            User? user = _context.Users.FirstOrDefault(user => user.Email == signUpDto.Email);

            if (user != null)
            {
                ViewBag.AlertMessage = "User alredy exist";
                return View(signUpDto);

            }


            if (user != null && user.Status == false)
            {
                ViewBag.AlertMessage = "User Blocked";
                return View(signUpDto);

            }

            Setting? roleSetting = _context.Settings.FirstOrDefault(setting => setting.Type == "ROLE" && setting.Value == "STUDENT");
            if (roleSetting == null)
            {
                ViewBag.AlertMessage = "Student Role Not Found";
                return View(signUpDto);
            }


            var hash = hashService.RandomHash();

            user = new()
            {
                Email = signUpDto.Email,
                DomainSettingId = domainSetting.SettingId,
                RoleSettingId = roleSetting.SettingId,
                Hash = hashService.HashPassword(signUpDto.Password),
                ConfirmToken = hash,
                Name = mailService.GetAddress(signUpDto.Email),
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            mailService.SendMailConfirm(signUpDto.Email, hash);
            ViewBag.AlertMessageSuccess = "An email has been sent to you, please confirm";
            return View(new SignUpDto());
        }

        [Route("sign-in")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SignIn(SignInDto signInDto, [FromServices] IHashService hashService)
        {
            if (!ModelState.IsValid) return View();

            User? user = _context.Users.Include(s => s.RoleSetting).FirstOrDefault(user => user.Email == signInDto.Email);

            if (user == null || !hashService.Verify(signInDto.Password, user.Hash!))
            {
                ViewBag.AlertMessage = "Please check email or password";
                return View(signInDto);
            }
            if (user != null && user.Status == false)
            {
                ViewBag.AlertMessage = "User blocked";
                return View(signInDto);
            }
            if (user != null && user.Status == null)
            {
                ViewBag.AlertMessage = "User alredy not confirm";
                return View(signInDto);
            }

            string userJsonString = JsonSerializer.Serialize(user!, new JsonSerializerOptions()
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            });

            HttpContext.Session.SetString("User", userJsonString);

            switch (user.RoleSetting.Value)
            {
                case "ADMIN":
                    return View("Views/Admin/AdminHome.cshtml");
                case "SUBJECT_MANAGER":
                    return View("Views/Manager/ManagerHome.cshtml");
                case "CLASS_MANAGER":
                    return View("Views/ClassManager/ClassManager.cshtml");
                    /* 
                     case "MENTOR":
                         return View("Views/Admin/AdminHome.cshtml");
                     case "STUDENT":
                         return View("Views/Admin/AdminHome.cshtml");*/
            }

            return View();
        }

        [Route("change-password")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthActionFilter]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto, [FromServices] IHashService hashService)
        {
            if (!ModelState.IsValid) return View();

            if (dto.NewPassword != dto.ConfirmPassword)
            {
                ViewBag.AlertMessage = "Password not match";
                return View(dto);
            }

            string userJsonString = HttpContext.Session.GetString("User")!;
            int userId = JsonSerializer.Deserialize<User>(userJsonString)!.Id;

            User? user = _context.Users.FirstOrDefault(user => user.Id == userId);

            if (user == null) return View(dto);

            if (!hashService.Verify(dto.OldPassword, user.Hash))
            {
                ViewBag.AlertMessage = "Password incorrect";
                return View(dto);
            }

            user.Hash = hashService.HashPassword(dto.NewPassword);
            await _context.SaveChangesAsync();
            ViewBag.AlertMessageSuccess = "Password was successfully changed";

            return View(new ChangePasswordDto());
        }

        [Route("forgot-password")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword( ForgotPasswordDto dto, [FromServices] IMailService mailService, [FromServices] IHashService hashService)
        {
            if (!ModelState.IsValid) return View();

            User? user = _context.Users.FirstOrDefault(user => user.Email == dto.Email && user.Hash != string.Empty);

            if (user == null)
            {
                ViewBag.AlertMessage = "Email invalid";
                return View(dto);
            }

            string hash = hashService.RandomHash();

            user.ResetPassToken = hash;
            await _context.SaveChangesAsync();
            mailService.SendResetPassword(dto.Email, hash);
            ViewBag.AlertMessageSuccess = "Please check your email";

            return View(new ForgotPasswordDto());
        }


        [Route("reset-password/{token}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword( ResetPasswordDto dto, [FromServices] IHashService hashService, string token)
        {
            if (!ModelState.IsValid) return View();

            if (dto.Password != dto.ConfirmPassword)
            {
                ViewBag.AlertMessage = "Password not match";
                return View(dto);
            }

            User? user = _context.Users.FirstOrDefault(user => user.ResetPassToken == token);

            if (user == null)
            {
                ViewBag.AlertMessage = "Token invalid";
                return View(dto);
            }

            user.ResetPassToken = null;
            user.Hash = hashService.HashPassword(dto.Password);

            await _context.SaveChangesAsync();
            ViewBag.AlertMessageSuccess = "Password has been reset successfully";

            return View(new ResetPasswordDto());
        }

        [Route("/logout")]
        [HttpPost]
        public IActionResult Logout()
        {

            HttpContext.Session.Remove("User");

            return RedirectToAction(nameof(SignIn));
        }
    }
}
