using G3.Views.Shared.Components.SearchBar;
using G3.Views.Shared.Components.SearchBar;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace G3.Controllers
{
    [AuthActionFilter]
    public class AdminController : Controller
    {
        private readonly SWPContext _context;

        public AdminController(SWPContext context)
        {
            _context = context;
        }


        [Route("/Admin/AdminHome")]
        public IActionResult AdminHome()
        {
            return View();
        }


        [Route("/Admin/RolesList")]
        public async Task<IActionResult> RolesList(string search, int pg = 1) //this is a combanation of RoleList and SearchRole though GET
        {

            var settings = await _context.Settings.Where(setting => setting.Type == "ROLE").ToListAsync();
            if (!string.IsNullOrEmpty(search))
            {

                settings = await _context.Settings
                    .Where(setting => setting.Type == "ROLE" && setting.Name.Contains(search))
                    .ToListAsync();
                // Pass the list of settings to the view.
            }


            const int pageSize = 5;
            if (pg < 1)
            {
                pg = 1;
            }

            int recsCount = settings.Count();

            var pager = new Pager(recsCount, pg, pageSize);

            int recSkip = (pg - 1) * pageSize;

            var data = settings.Skip(recSkip).Take(pager.PageSize).ToList();

            SPager searchPager = new SPager(recsCount, pg, pageSize)
            {
                Action = "RolesList",
                Controller = "Admin",
                SearchText = search,
            };

            this.ViewBag.Pager = pager;

            ViewBag.SearchString = search;
            ViewBag.SearchPager = searchPager;
            // Pass the list of settings to the view.
            return View("/Views/Admin/RolesList.cshtml", data);
        }

        //GET
        [Route("/Admin/RolesEdit")]
        public IActionResult Edit(int? id) //edit of system's roles, dont mistake for user edit
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var settingFromDB = _context.Settings.Find(id);
            if (settingFromDB == null) return NotFound();

            settingFromDB.IsActive = !settingFromDB.IsActive;
            _context.SaveChanges();
            return RedirectToAction("RolesList");
        }

        //POST
        [Route("/Admin/RolesEdit")]
        [HttpPost]
        [ValidateAntiForgeryToken] // idk what this does, prevent cross-site attack or smt
        public IActionResult Edit(Setting obj) //edit of system's roles, dont mistake for user edit
        {
            if (ModelState.IsValid)
            {
                _context.Settings.Update(obj);
                _context.SaveChanges();
                //TempData["success"] = "Category update successfully";

                return RedirectToAction("RolesList");
            }
            return View("RolesEdit", obj);
        }


        [Route("/Admin/RoleCreate")]
        public IActionResult RoleCreate()
        {
            return View("/Views/Admin/RoleCreate.cshtml");
        }

        //POST
        [Route("/Admin/RoleCreate")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RoleCreate(Setting obj)
        {
            if (ModelState.IsValid)
            {

                string[] words = obj.Name.Split(' ');
                string value = string.Join("_", words).ToUpper();
                obj.Value = value;
                obj.Type = "ROLE";
                obj.IsActive = true;
                _context.Settings.Add(obj);
                _context.SaveChanges();
                //TempData["success"] = "Category created successfully";
                return RedirectToAction("RolesList");
            }
            return View("/Views/Admin/RolesList.cshtml", obj);
        }

        [Route("/Admin/UsersRoleList")]
        public async Task<IActionResult> UsersRoleList()
        {
            var userSetting = await _context.Users.Include(u => u.DomainSetting).Include(u => u.RoleSetting).ToListAsync();
            return View("/Views/Admin/UsersRoleList.cshtml", userSetting);
        }

        [Route("/Admin/UsersRoleEdit")]
        public async Task<IActionResult> UsersRoleEdit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();

            }

            var user = await _context.Users.FindAsync(id);
            var settings = await _context.Settings.Where(s => s.Type == "ROLE") // Filter settings where Type is "ROLE"
                                                  .ToListAsync();
            if (user == null)
            {
                return NotFound();
            }
            UserSettingViewModel userSettingViewModel = new UserSettingViewModel()
            {
                User = user,
                Settings = settings
            };

            // Pass the list to the view.
            return View("/Views/Admin/UsersRoleEdit.cshtml", userSettingViewModel);
        }

        [Route("/Admin/UsersRoleEdit")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> UsersRoleEdit(UserSettingViewModel obj)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.Find(obj.User.Id);
                if (user != null)
                {
                    user.RoleSettingId = obj.User.RoleSettingId;
                    _context.SaveChanges();
                }

                //TempData["success"] = "Category update successfully";

                return RedirectToAction("UsersRoleList");
            }
            return View("UsersRoleEdit", obj);
        }


    }
}
