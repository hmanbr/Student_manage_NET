using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using G3.Models;
using NuGet.Protocol;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace G3.Controllers {
    [Route("/auth")]
    public class AuthController : Controller {
        private readonly SWPContext _context;

        public AuthController(SWPContext context) {
            _context = context;
        }

        [Route("sign-up")]
        public IActionResult SignUp() {
            return View();
        }

        [Route("sign-in")]
        public IActionResult SignIn() {
            return View();
        }

        [Route("change-password")]
        public IActionResult ChangePassword() {
            return View();
        }


        [Route("forgot-password")]
        public IActionResult ForgotPassword() {
            return View();
        }

        [Route("reset-password")]
        public IActionResult ResetPassword() {
            return View();
        }

        [Route("sign-up")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp([Bind("Email,Password,ConfirmPassword,Name,DateOfBirth,Phone,Address,Gender")] SignUpDto signUpDto, [FromServices] IMailService mailService, [FromServices] IHashService hashService) {
            if (signUpDto.Password != signUpDto.ConfirmPassword) {
                ViewBag.AlertMessage = "Password not match";
                return View();
            }

            string? domain = mailService.GetDomain(signUpDto.Email);
            if (domain == null) {
                ViewBag.AlertMessage = "Email Domain False";
                return View();
            }

            Setting? domainSetting = _context.Settings.FirstOrDefault(setting => setting.Type == "DOMAIN" && setting.Value == domain);
            if (domainSetting == null) {
                ViewBag.AlertMessage = "Email Domain Not Accept";
                return View();
            }

            User? user = _context.Users.FirstOrDefault(user => user.Email == signUpDto.Email);

            if (user != null && user.Blocked) {
                ViewBag.AlertMessage = "User Blocked";
            }

            if (user != null && user.Confirmed) {
                ViewBag.AlertMessage = "Email already used";
            }

            Setting? roleSetting = _context.Settings.FirstOrDefault(setting => setting.Type == "ROLE" && setting.Value == "STUDENT");
            if (roleSetting == null) {
                ViewBag.AlertMessage = "Student Role Not Found";
                return View();
            }


            var hash = hashService.RandomHash();

            if (ModelState.IsValid) {
                user = new() {
                    Email = signUpDto.Email,
                    DomainSettingId = domainSetting.SettingId,
                    RoleSettingId = roleSetting.SettingId,
                    Hash = hashService.HashPassword(signUpDto.Password),
                    ConfirmToken = hash,
                    Name = signUpDto.Name,
                    DateOfBirth = signUpDto.DateOfBirth,
                    Phone = signUpDto.Phone,
                    Address = signUpDto.Address,
                    Gender = signUpDto.Gender,
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                mailService.SendMailConfirm(signUpDto.Email, hash);
                return View(signUpDto);

            }
            return View(signUpDto);
        }

        [Route("sign-in")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SignIn([Bind("Email,Password")] SignInDto signInDto, [FromServices] IHashService hashService) {
            User? user = _context.Users.FirstOrDefault(user => user.Email == signInDto.Email);

            if (user == null) {
                ViewBag.AlertMessage = "Please check email or password";
                return View();
            }
            if (user != null && user.Blocked) {
                ViewBag.AlertMessage = "User blocked";
                return View();
            }
            if (user != null && !user.Confirmed) {
                ViewBag.AlertMessage = "User alredy not confirm";
                return View();
            }

            string userJsonString = JsonSerializer.Serialize(user!);
            HttpContext.Session.SetString("User", userJsonString);
            return View();
        }

        [Route("change-password")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword([Bind("OldPassword,NewPassword,ConfirmPassword")] ChangePasswordDto changePasswordDto) {
            string? userJsonString = HttpContext.Session.GetString("User");
            if (userJsonString == null) {
                return SignIn();
            }

            User? user = JsonSerializer.Deserialize<User>(userJsonString);
            if (user == null) {
                return SignIn();
            }

            
            return View();
        }
    }
}
