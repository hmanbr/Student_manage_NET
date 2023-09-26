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
    public class SubjectsController : Controller
    {
        private readonly SWPContext _context;

        public SubjectsController(SWPContext context)
        {
            _context = context;
        }

        // GET: Subjects
        [Route("/Subjects/Index")]
        public async Task<IActionResult> Index()
        {
            var sWPContext = _context.Subjects.Include(m => m.Mentor);
            
           
            return View(await sWPContext.ToListAsync());
        }
        [Route("/Subjects/Index")]
        [HttpPost]
        public async Task<IActionResult> Index(string search)
        {
            ViewData["search"] = search;
            var SearchQuery = from x in _context.Subjects select x;
            if (!String.IsNullOrEmpty(search))
            {
                SearchQuery = SearchQuery.Where(x => x.SubjectCode.Contains(search)).Include(m => m.Mentor);
            }
            return View(await SearchQuery.ToListAsync());
        }

        // GET: Subjects/Details/5
        [Route("/Subjects/Details")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Subjects == null)
            {
                return NotFound();
            }

            var subject = await _context.Subjects
                .Include(s => s.Mentor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subject == null)
            {
                return NotFound();
            }

            return View(subject);
        }

        // GET: Subjects/Create
        [Route("/Subjects/Create")]
        public IActionResult Create()
        {
            ViewData["MentorId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Subjects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("/Subjects/Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SubjectCode,Name,Status,MentorId")] Subject s)
        {
            if (_context.Subjects.Any(p => p.SubjectCode == s.SubjectCode))
            {
                ViewData["exist"] = "The subject code already exists";
                ViewData["MentorId"] = new SelectList(_context.Users, "Id", "Id");
                return View();
               
            }
            _context.Add(s);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            
            
        }

        // GET: Subjects/Edit/5
        [Route("/Subjects/Edit")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Subjects == null)
            {
                return NotFound();
            }

            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null)
            {
                return NotFound();
            }
            ViewData["MentorId"] = new SelectList(_context.Users, "Id", "Id", subject.MentorId);
            return View(subject);
        }

        // POST: Subjects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("/Subjects/Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SubjectCode,Name,Status,MentorId")] Subject subject)
        {
            if (id != subject.Id)
            {
                return NotFound();
            }
            if (_context.Subjects.Any(p => p.SubjectCode == subject.SubjectCode &&  p.Id != id))
            {
                ViewData["exist"] = "The subject code already exists";
                ViewData["MentorId"] = new SelectList(_context.Users, "Id", "Id");
                return View();

            }
            else {

                try
                {
                    _context.Update(subject);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubjectExists(subject.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }

                }
            }
            //ViewData["MentorId"] = new SelectList(_context.Users, "Id", "Id", subject.MentorId);
            return RedirectToAction(nameof(Index));
        }

        // GET: Subjects/Delete/5
        [Route("/Subjects/Delete")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Subjects == null)
            {
                return NotFound();
            }

            var subject = await _context.Subjects
                .Include(s => s.Mentor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subject == null)
            {
                return NotFound();
            }

            return View(subject);
        }

        [Route("/Subjects/Delete")]
        // POST: Subjects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Subjects == null)
            {
                return Problem("Entity set 'SWPContext.Subjects'  is null.");
            }
            var subject = await _context.Subjects.FindAsync(id);
            if (subject != null)
            {
                _context.Subjects.Remove(subject);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SubjectExists(int id)
        {
          return (_context.Subjects?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
