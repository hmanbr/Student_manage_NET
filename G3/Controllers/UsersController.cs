using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace G3.Controllers
{
    public class UsersController : Controller
    {
        private readonly SWPContext _context;

        public UsersController(SWPContext context)
        {
            _context = context;
        }

        // GET: Users


        // GET: Users/Details/5


        // GET: Users/Create
        public IActionResult Create()
        {
            ViewData["DomainSettingId"] = new SelectList(_context.Settings, "SettingId", "SettingId");
            ViewData["RoleSettingId"] = new SelectList(_context.Settings, "SettingId", "SettingId");
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Email,DomainSettingId,RoleSettingId,Hash,Confirmed,Blocked,ConfirmToken,ConfirmTokenVerifyAt,ResetPassToken,Avatar,Name,DateOfBirth,Phone,Address,Gender,CreatedAt,UpdatedAt")] User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DomainSettingId"] = new SelectList(_context.Settings, "SettingId", "SettingId", user.DomainSettingId);
            ViewData["RoleSettingId"] = new SelectList(_context.Settings, "SettingId", "SettingId", user.RoleSettingId);
            return View(user);
        }

        // GET: Users/Edit/5
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
            ViewData["DomainSettingId"] = new SelectList(_context.Settings, "SettingId", "SettingId", user.DomainSettingId);
            ViewData["RoleSettingId"] = new SelectList(_context.Settings, "SettingId", "SettingId", user.RoleSettingId);
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Email,DomainSettingId,RoleSettingId,Hash,Confirmed,Blocked,ConfirmToken,ConfirmTokenVerifyAt,ResetPassToken,Avatar,Name,DateOfBirth,Phone,Address,Gender,CreatedAt,UpdatedAt")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
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

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.DomainSetting)
                .Include(u => u.RoleSetting)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
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
