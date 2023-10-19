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
    public class SubjectsettingsController : Controller
    {
        private readonly SWPContext _context;

        public SubjectsettingsController(SWPContext context)
        {
            _context = context;
        }
        /*[Route("/Subjectsettings/SubjectSettingList")]
        // GET: Subjectsettings
        public async Task<IActionResult> SubjectSettingList(int page = 1, int pageSize = 5)
        {
            var query = _context.Subjectsettings.Include(s => s.Subject);
            var totalItems = query.Count();

            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            if (page < 1)
            {
                //test
                page = 1;
            }
            else if (page > totalPages)
            {
                page = totalPages;
            }

            var s = query

                .Skip((page - 1) * pageSize)
                .Take(pageSize).ToList();

            ViewBag.TotalItems = totalItems;
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;

            return View(s);
        }*/

        [Route("/Subjectsettings/SubjectSettingList")]
        
        public async Task<IActionResult> SubjectSettingList(string search, string SortBy, int page = 1, int pageSize = 5)
        {

           
            var Query = _context.Subjectsettings.Include(s => s.Subject);

            ViewData["search"] = search;

            if (!String.IsNullOrEmpty(search))
            {
                Query = Query.Where(x => x.Subject.SubjectCode.Contains(search) || x.Subject.Name.Contains(search)).Include(m => m.Subject);

            }

            ViewData["Sort"] = SortBy;
            if (SortBy == "ASC")
            {
                Query = Query.OrderBy(x => x.Subject.SubjectCode).Include(m => m.Subject);

            }
            else
            {
                Query = Query.OrderByDescending(x => x.Subject.SubjectCode).Include(m => m.Subject);

            }

            var totalItems = Query.Count();

            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            if (page < 1)
            {
                //test
                page = 1;
            }
            else if (page > totalPages)
            {
                page = totalPages;
            }

            Query = Query

                .Skip((page - 1) * pageSize)
                .Take(pageSize).Include(m => m.Subject);
            ViewBag.TotalItems = totalItems;
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;

            return View(await Query.ToListAsync());
        }
        /*[Route("/Subjectsettings/SubjectSettingList")]
        [HttpPost]
        public async Task<IActionResult> SubjectSettingList(string search, string SortBy, int page = 1, int pageSize = 5)
        {
            var Query = _context.Subjectsettings.AsQueryable();

            ViewData["search"] = search;

            if (!String.IsNullOrEmpty(search))
            {
                Query = Query.Where(x => x.Subject.SubjectCode.Contains(search) || x.Subject.Name.Contains(search)).Include(m => m.Subject);

            }

            ViewData["Sort"] = SortBy;
            if (SortBy == "ASC")
            {
                Query = Query.OrderBy(x => x.Subject.SubjectCode).Include(m => m.Subject);

            }
            else
            {
                Query = Query.OrderByDescending(x => x.Subject.SubjectCode).Include(m => m.Subject);

            }

            var totalItems = Query.Count();

            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            if (page < 1)
            {
                //test
                page = 1;
            }
            else if (page > totalPages)
            {
                page = totalPages;
            }

            Query = Query

                .Skip((page - 1) * pageSize)
                .Take(pageSize).Include(m => m.Subject);
            ViewBag.TotalItems = totalItems;
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            //var sWPContext = _context.Subjectsettings.Include(s => s.Subject);
            return View(await Query.ToListAsync());
        }*/


        // GET: Subjectsettings/Create
        [Route("/Subjectsettings/Create")]
        public IActionResult SubjectSettingAdd()
        {
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "SubjectCode");
            return View();
        }

        // POST: Subjectsettings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("/Subjectsettings/Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubjectSettingAdd([Bind("Id,Value,Description,SubjectId")] Subjectsetting subjectsetting)
        {
            if (_context.Subjectsettings.Any(s => s.SubjectId == subjectsetting.SubjectId))
            {
                ViewData["exist"] = "This subject has been setup";
                ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "SubjectCode", subjectsetting.SubjectId);
                return View(subjectsetting);
            }

            _context.Add(subjectsetting);
            TempData["SuccessMessage"] = "Create Successful";
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(SubjectSettingList));
        }

        // GET: Subjectsettings/Edit/5
        [Route("/Subjectsettings/Edit")]
        public async Task<IActionResult> SubjectSettingEdit(int? id)
        {
            if (id == null || _context.Subjectsettings == null)
            {
                return NotFound();
            }
            var subjectsetting = await _context.Subjectsettings.FindAsync(id);
            
            if (subjectsetting == null)
            {
                return NotFound();
            }
            
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "SubjectCode", subjectsetting.SubjectId);
           
            return View(subjectsetting);
            
        }

        // POST: Subjectsettings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("/Subjectsettings/Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubjectSettingEdit(int id, [Bind("Id,Value,Description,SubjectId")] Subjectsetting subjectsetting)
        {
            if (id != subjectsetting.Id)
            {
                return NotFound();
            }

            if (_context.Subjectsettings.Any(p => p.SubjectId == subjectsetting.SubjectId && p.Id != id))
            {
                ViewData["exist"] = "This subject has been setup";
                ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "SubjectCode", subjectsetting.SubjectId);
                return View();
            }
            else
            {
                try
                {
                    _context.Update(subjectsetting);
                    TempData["SuccessMessageEdit"] = "Update Successful.";
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubjectsettingExists(subjectsetting.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(SubjectSettingList));
            }
        }
        [Route("/Subjectsettings/EditSS_toggle")]
        public IActionResult SubjectSetting_toggle(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var SS = _context.Subjects.Find(id);
            if (SS == null)
            {
                return NotFound();
            }
                 SS.Status = !SS.Status;
            _context.SaveChanges();
            return RedirectToAction("SubjectSettingList");
        }
        private bool SubjectsettingExists(int id)
        {
          return (_context.Subjectsettings?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
