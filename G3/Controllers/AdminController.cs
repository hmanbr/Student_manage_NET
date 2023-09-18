using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using G3.Models;

namespace G3.Controllers
{
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

        [Route("/Admin/UserList")]
        public async Task<IActionResult> UsersList()
        {
            var sWPContext = _context.Users.Include(u => u.DomainSetting).Include(u => u.RoleSetting);
            return View("/Views/Admin/UsersList.cshtml", await sWPContext.ToListAsync());
        }

        [Route("/Admin/Details")]
        public async Task<IActionResult> Details(int? id)
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

            return View("/Views/Admin/Details.cshtml", user);
        }

        [Route("/Admin/RolesList")]
        public async Task<IActionResult> RolesList()
        {
            var sWPContext = _context.Users.Include(u => u.DomainSetting).Include(u => u.RoleSetting);
            return View("/Views/Admin/RolesList.cshtml", await sWPContext.ToListAsync());
        }
    }
}
