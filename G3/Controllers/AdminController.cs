using Microsoft.AspNetCore.Mvc;

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

        [Route("/")]
        public IActionResult AdminHome()
        {
            return View();
        }

        [Route("/Admin/RolesList")]
        public async Task<IActionResult> RolesList()
        {
			var settings = await _context.Settings.Where(setting => setting.Type == "ROLE").ToListAsync();
			// Pass the list of settings to the view.
			return View("/Views/Admin/RolesList.cshtml", settings);
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
			if (settingFromDB == null)
			{
				return NotFound();
			}
			return View("RolesEdit", settingFromDB);
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
    }
}
