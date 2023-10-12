using G3.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Drawing.Printing;

namespace G3.Controllers
{
    /*[AuthActionFilter]*/
    public class SettingsController : Controller
    {
        private readonly SWPContext _context;

        public SettingsController(SWPContext context)
        {
            _context = context;
        }

        // GET: Settings

        [Route("/admin/listEmailDM")]
        public async Task<IActionResult> ListEmailDM()
        {
            return _context.Settings != null ?
                        View(await _context.Settings.ToListAsync()) :
                        Problem("Entity set 'SWPContext.Settings'  is null.");
        }

        [Route("/admin/listEmailDM")]
        [HttpPost]
        public async Task<IActionResult> Search_ListEmailDM(string search)
        {
            ViewData["searchString"] = search;
            var email_name = from n in _context.Settings select n;

            if (!string.IsNullOrEmpty(search))
            {
                email_name = email_name.Where(n => n.Value.Contains(search));
               
            }
            return View("Views/Settings/listEmailDM.cshtml", await email_name.ToArrayAsync());

        }

        [Route("/admin/Sort_listEmailDM")]
        public async Task<IActionResult> Sort_ListEmailDM(string sortOrder)
        {
            ViewData["SortByValue"] = sortOrder;
            var email_name = from n in _context.Settings select n;
            var num_email = _context.Settings.ToList();

            /* ViewData["SortByValue"] = String.IsNullOrEmpty(sortOrder) ? "sortValue" : "";
             ViewData["SortByStatus"] = sortOrder == "Isactive" ? "status" : "status";

             switch (sortOrder)
             {

                 case "sortValue":
                     email_name = email_name.OrderBy(n => n.Value);
                     break;
                 case "status":
                     email_name = email_name.OrderByDescending(n => n.IsActive);
                     break;
             }*/

            ViewBag.TotalPage = Math.Ceiling(num_email.Count() / 3.0);

            email_name = email_name.OrderBy(n => n.Value);

            return View("Views/Settings/listEmailDM.cshtml", await email_name.ToArrayAsync());

        }

        [Route("/admin/SortST_listEmailDM")]
        public async Task<IActionResult> SortST_ListEmailDM(string sortOrder)
        {
            ViewData["SortByStatus"] = sortOrder;
            var email_name = from n in _context.Settings select n;

            email_name = email_name.OrderBy(n => n.IsActive);

            return View("Views/Settings/listEmailDM.cshtml", await email_name.ToArrayAsync());

        }


        // GET: Settings/Details/5
        [Route("/admin/DetailsEmailDM")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Settings == null)
            {
                return NotFound();
            }

            var setting = await _context.Settings
                .FirstOrDefaultAsync(m => m.SettingId == id);
            if (setting == null)
            {
                return NotFound();
            }

            return View(setting);
        }

        // GET: Settings/Create
        [Route("/admin/createEmailDM")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Settings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("/admin/createEmailDM")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SettingId,Type,Name,Value,IsActive")] Setting setting)
        {

            if (ModelState.IsValid)
            {
                setting.Type = "DOMAIN";

                if (_context.Settings.Any(p=> p.Value == setting.Value))
                {
                    ViewData["exist"] = "The value already exists";
                    return View();

                }

                _context.Add(setting);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ListEmailDM));
            }
            return View(setting);
        }

        // GET: Settings/Edit/5
        [Route("/admin/editEmailDM")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Settings == null)
            {
                return NotFound();
            }

            var setting = await _context.Settings.FindAsync(id);
            if (setting == null)
            {
                return NotFound();
            }
            return View(setting);
        }

        // POST: Settings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [Route("/admin/editEmailDM")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit_confirm([Bind("SettingId,Type,Name,Value,IsActive")] Setting setting)
        {
            if (ModelState.IsValid)
            {
                try
                {


                    _context.Update(setting);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SettingExists(setting.SettingId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(ListEmailDM));
            }
            return View(setting);
        }

        // GET: Settings/Delete/5
        [Route("/admin/deleteEmailDM")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Settings == null)
            {
                return NotFound();
            }

            var setting = await _context.Settings
                .FirstOrDefaultAsync(m => m.SettingId == id);
            if (setting == null)
            {
                return NotFound();
            }

            return View(setting);
        }

        // POST: Settings/Delete/5
        [Route("/admin/deleteEmailDM")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Settings == null)
            {
                return Problem("Entity set 'SWPContext.Settings'  is null.");
            }
            var setting = await _context.Settings.FindAsync(id);
            if (setting != null)
            {
                _context.Settings.Remove(setting);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ListEmailDM));
        }

        private bool SettingExists(int id)
        {
            return (_context.Settings?.Any(e => e.SettingId == id)).GetValueOrDefault();
        }


        [Route("/admin/editEmailDM_toggle")]
        public IActionResult Edit_toggle(int? id)
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
            settingFromDB.IsActive = !settingFromDB.IsActive;
            _context.SaveChanges();
            return RedirectToAction("listEmailDM");
        }


    }
}