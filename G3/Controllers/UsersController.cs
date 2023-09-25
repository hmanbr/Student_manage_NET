using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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

        [Route("/Admin/UserList")]
        public async Task<IActionResult> Index()
        {
            var sWPContext = _context.Users.Include(u => u.DomainSetting).Include(u => u.RoleSetting);
            return View("/Views/Users/Index.cshtml", await sWPContext.ToListAsync());
        }

        [Route("/Admin/Details")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

        // GET: Users/Details/5


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
        public async Task<IActionResult> Create([Bind("Id,Email,DomainSettingId,RoleSettingId,Hash,Confirmed,Blocked,ConfirmToken,ConfirmTokenVerifyAt,ResetPassToken,Avatar,Name,DateOfBirth,Phone,Address,Gender,CreatedAt,UpdatedAt")] User user)
        {
            if (ModelState.IsValid)
            {
                var uploadedFile = HttpContext.Request.Form.Files["AvatarFile"];
                if (uploadedFile != null && uploadedFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "avatars");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + uploadedFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await uploadedFile.CopyToAsync(fileStream);
                    }

                    user.Avatar = "/avatars/" + uniqueFileName;
                }
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var DomainSettings = _context.Settings.Where(ds => ds.Type == "DOMAIN").ToList();
            ViewData["DomainSettingId"] = new SelectList(DomainSettings, "SettingId", "Value");
            var RoleSettings = _context.Settings.Where(rs => rs.Type == "ROLE").ToList();
            ViewData["RoleSettingId"] = new SelectList(RoleSettings, "SettingId", "Value");


            return View("/Views/Users/Create.cshtml", user);
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
            var DomainSettings = _context.Settings.Where(ds => ds.Type == "DOMAIN").ToList();
            ViewData["DomainSettingId"] = new SelectList(DomainSettings, "SettingId", "Value");
            var RoleSettings = _context.Settings.Where(rs => rs.Type == "ROLE").ToList();
            ViewData["RoleSettingId"] = new SelectList(RoleSettings, "SettingId", "Value");
            return View("/Views/Users/Edit.cshtml", user);
        }

        [HttpPost]
        [Route("/Admin/Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Email,DomainSettingId,RoleSettingId,Hash,Confirmed,Blocked,ConfirmToken,ConfirmTokenVerifyAt,ResetPassToken,Avatar,Name,DateOfBirth,Phone,Address,Gender,CreatedAt,UpdatedAt")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
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
                try
                {
                    _context.Update(user);
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
            ViewData["DomainSettingId"] = new SelectList(_context.Settings, "SettingId", "SettingId", user.DomainSettingId);
            ViewData["RoleSettingId"] = new SelectList(_context.Settings, "SettingId", "SettingId", user.RoleSettingId);
            return View(user);
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
