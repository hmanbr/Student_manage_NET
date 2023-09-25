using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace G3.Controllers
{
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
    }
}
