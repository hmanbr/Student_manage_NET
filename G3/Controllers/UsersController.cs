using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Controller;
using NuGet.Packaging;
using Org.BouncyCastle.Asn1.Ocsp;

namespace G3.Controllers
{
    public class UsersController : Controller
    {
        private readonly SWPContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public UsersController(SWPContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        [Route("/Admin/UserList")]
        public async Task<IActionResult> Index()
        {
            var sWPContext = await _context.Users.Include(u => u.DomainSetting).Include(u => u.RoleSetting).ToListAsync();
            var RoleSettings = _context.Settings.Where(rs => rs.Type == "ROLE").ToList();
            ViewData["RoleSettingId"] = new SelectList(RoleSettings, "SettingId", "Value");
            return View("/Views/Users/Index.cshtml", sWPContext);
        }

        [HttpGet]
        [Route("/Admin/Search")]
        public IActionResult Search(string searchName, int? searchRole, int? searchStatus)
        {
            var sWPContext = _context.Users.Include(u => u.DomainSetting).Include(u => u.RoleSetting).ToList();
            var RoleSettings = _context.Settings.Where(rs => rs.Type == "ROLE").ToList();
            ViewData["RoleSettingId"] = new SelectList(RoleSettings, "SettingId", "Value");

            if (searchRole.HasValue && searchRole != 0)
            {
                sWPContext = sWPContext.Where(item => item.RoleSettingId == searchRole).ToList();
            }

            if (!string.IsNullOrEmpty(searchName))
            {
                sWPContext = sWPContext.Where(item => item.Name.Contains(searchName, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (searchStatus.HasValue)
            {
                bool? status = null;
                switch (searchStatus)
                {
                    case 1:
                        {
                            status = true;
                            break;
                        }
                    case -1:
                        {
                            status = null;
                            break;
                        }
                    case 0:
                        {
                            status = false;
                            break;
                        }
                }
                sWPContext = sWPContext.Where(item => item.Status == status).ToList();
            }
            ViewBag.SearchName = searchName;
            ViewBag.SearchRole = searchRole;
            ViewBag.SearchStatus = searchStatus;
            return View("/Views/Users/Index.cshtml", sWPContext);
        }

        [Route("/Admin/Details")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var DomainSettings = _context.Settings.Where(ds => ds.Type == "DOMAIN").ToList();
            ViewData["DomainSettingId"] = new SelectList(DomainSettings, "SettingId", "Value");
            var RoleSettings = _context.Settings.Where(rs => rs.Type == "ROLE").ToList();
            ViewData["RoleSettingId"] = new SelectList(RoleSettings, "SettingId", "Value");
            return View("/Views/Users/Details.cshtml", user);
        }

        [HttpGet]
        [Route("/Admin/Create")]
        public IActionResult Create()
        {
            var DomainSettings = _context.Settings.Where(ds => ds.Type == "DOMAIN").ToList();
            ViewData["DomainSettingId"] = new SelectList(DomainSettings, "SettingId", "Value");
            var RoleSettings = _context.Settings.Where(rs => rs.Type == "ROLE").ToList();
            ViewData["RoleSettingId"] = new SelectList(RoleSettings, "SettingId", "Value");
            return View("/Views/Users/Create.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/Admin/Create")]
        public async Task<IActionResult> Create([Bind("Email,Avatar,RoleSettingId,Status,Name,Phone,CreatedAt,UpdatedAt")] User user, [FromServices] IHashService hashService, [FromServices] IMailService mailService)
        {
            var DomainSettings = _context.Settings.Where(ds => ds.Type == "DOMAIN").ToList();
            ViewData["DomainSettingId"] = new SelectList(DomainSettings, "SettingId", "Value");
            var RoleSettings = _context.Settings.Where(rs => rs.Type == "ROLE").ToList();
            ViewData["RoleSettingId"] = new SelectList(RoleSettings, "SettingId", "Value");

            //check if email domain is valid (exist and is active)
            String domain = user.Email.Split('@')[1];
            Boolean isValidEmail = false;
            foreach (var item in DomainSettings)
            {
                if (string.Equals(domain, item.Value, StringComparison.OrdinalIgnoreCase) && item.IsActive == true)
                {
                    user.DomainSettingId = item.SettingId;
                    isValidEmail = true;
                    break;
                }
            }
            //check if there is another user that use the same email.
            var isEmailInUse = _context.Users.Any(u => u.Email == user.Email);
            //if email is invalid, return
            if (!isValidEmail)
            {
                ViewData["ErrorMessage"] = new String("Invalid email domain.");
                return View(user);
            }
            //if email is used by another user, return
            if (isEmailInUse)
            {
                ViewData["ErrorMessage"] = new String("This email has been used.");
                return View(user);
            }
            if (ModelState.IsValid)
            {
                //generate a random 8-char long password and hash it.
                String randomPass = hashService.RandomStringGenerator(8);
                user.Hash = hashService.HashPassword(randomPass);
                //save user avatar
                IFormFile avatarFile = Request.Form.Files["AvatarFile"];
                if (avatarFile != null && avatarFile.Length > 0)
                {
                    string avatarPath = Path.Combine(_webHostEnvironment.WebRootPath, "avatars");

                    if (!Directory.Exists(avatarPath))
                    {
                        Directory.CreateDirectory(avatarPath);
                    }

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(avatarFile.FileName);

                    string filePath = Path.Combine(avatarPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await avatarFile.CopyToAsync(stream);
                    }
                    user.Avatar = "/avatars/" + fileName;
                }
                //user status set as active
                user.Status = true;
                //add new user and send password to user if successful
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                    mailService.SendPassword(user.Email, randomPass);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(user);
            }
        }

        [HttpGet]
        [Route("/Admin/Edit")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            UpdateUserDto userInfo = new UpdateUserDto(user);
            var DomainSettings = _context.Settings.Where(ds => ds.Type == "DOMAIN").ToList();
            ViewData["DomainSettingId"] = new SelectList(DomainSettings, "SettingId", "Value");
            var RoleSettings = _context.Settings.Where(rs => rs.Type == "ROLE").ToList();
            ViewData["RoleSettingId"] = new SelectList(RoleSettings, "SettingId", "Value");
            return View("/Views/Users/Edit.cshtml", userInfo);
        }

        [HttpPost]
        [Route("/Admin/Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Status,Avatar,Name,DateOfBirth,Phone,Address,Gender,Description,UpdatedAt")] UpdateUserDto user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }
            var DomainSettings = _context.Settings.Where(ds => ds.Type == "DOMAIN").ToList();
            ViewData["DomainSettingId"] = new SelectList(DomainSettings, "SettingId", "Value");
            var RoleSettings = _context.Settings.Where(rs => rs.Type == "ROLE").ToList();
            ViewData["RoleSettingId"] = new SelectList(RoleSettings, "SettingId", "Value");
            if (ModelState.IsValid)
            {
                //save user avatar
                IFormFile avatarFile = Request.Form.Files["AvatarFile"];
                if (avatarFile != null && avatarFile.Length > 0)
                {
                    string avatarPath = Path.Combine(_webHostEnvironment.WebRootPath, "avatars");

                    if (!Directory.Exists(avatarPath))
                    {
                        Directory.CreateDirectory(avatarPath);
                    }

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(avatarFile.FileName);

                    string filePath = Path.Combine(avatarPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await avatarFile.CopyToAsync(stream);
                    }
                    user.Avatar = "/avatars/" + fileName;
                }
                //update user
                try
                {
                    var entity = await _context.Users.SingleOrDefaultAsync(r => r.Id == user.Id);
                    if (entity != null)
                    {
                        user.saveData(entity);
                    }
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(user);
            }
        }

        [HttpPost]
        [Route("/Admin/Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'SWPContext.Users'  is null.");
            }
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
