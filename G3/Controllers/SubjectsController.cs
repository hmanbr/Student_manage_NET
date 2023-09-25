using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using G3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

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
        [Route("/Subject/ListSubject")]
        public async Task<IActionResult> ListSubject()
        {
            var sWPContext = _context.Subjects.Include(s => s.Manager);
            return View(await sWPContext.ToListAsync());
        }
      


        // GET: Subjects/Details/5
        [Route("/Subject/Details")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Subjects == null)
            {
                return NotFound();
            }

            var subject = await _context.Subjects
                .Include(s => s.Manager)
                .FirstOrDefaultAsync(m => m.SubjectCode == id);
            if (subject == null)
            {
                return NotFound();
            }

            return View(subject);
        }

        // GET: Subjects/Create
        [Route("/Subject/Create")]
        public IActionResult Create()
        {
            ViewData["ManagerId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Subjects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        /*  [Route("/Subject/Create")]
          [HttpPost]
          [ValidateAntiForgeryToken]
          public async Task<IActionResult> Create([Bind("SubjectCode,Name,Status,ManagerId")] Subject subject)
          {
              if (ModelState.IsValid)
              {
                  _context.Add(subject);
                  await _context.SaveChangesAsync();
                  return RedirectToAction(nameof(ListSubject));
              }
              ViewData["ManagerId"] = new SelectList(_context.Users, "Id", "Id", subject.ManagerId);
              return View(subject);
          }*/
        [Route("/Subject/Create")]
        [HttpPost]
        public IActionResult Create(Subject s)
        {
            try
            {
                _context.Subjects.Add(s);
                _context.SaveChanges();
                //ViewData["ManagerId"] = new SelectList(_context.Users, "Id", "Id", s.ManagerId);
                
                return RedirectToAction(nameof(ListSubject));
                //return View(ListSubject);
            }
            catch (DbUpdateConcurrencyException ex)
            {
               
                Console.WriteLine("The subject code already exists.");
                return View();
            }
        }

        // GET: Subjects/Edit/5
        [Route("/Subject/Edit")]
        public async Task<IActionResult> Edit(string id)
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
            ViewData["ManagerId"] = new SelectList(_context.Users, "Id", "Id", subject.ManagerId);
            return View(subject);
        }

        // POST: Subjects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        /*[Route("/Subject/Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("SubjectCode,Name,Status,ManagerId")] Subject subject)
        {
            if (id != subject.SubjectCode)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(subject);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubjectExists(subject.SubjectCode))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(ListSubject));
            }
            ViewData["ManagerId"] = new SelectList(_context.Users, "Id", "Id", subject.ManagerId);
            return View(subject);
        }*/
        [Route("/Subject/Edit")]
        [HttpPost]
        public IActionResult Edit(Subject s)
        {
            var sub = _context.Subjects.Where(x=>x.SubjectCode==s.SubjectCode).FirstOrDefault();
            if(sub != null)
            {
                sub.SubjectCode = s.SubjectCode;
                sub.Name = s.Name;
                sub.Status = s.Status;
                sub.ManagerId = s.ManagerId;
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(ListSubject));
        }

        // GET: Subjects/Delete/5
        [Route("/Subject/Delete")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Subjects == null)
            {
                return NotFound();
            }

            var subject = await _context.Subjects
                .Include(s => s.Manager)
                .FirstOrDefaultAsync(m => m.SubjectCode == id);
            if (subject == null)
            {
                return NotFound();
            }

            return View(subject);
        }

        // POST: Subjects/Delete/5
        [Route("/Subject/Delete")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
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
            return RedirectToAction(nameof(ListSubject));
        }

        private bool SubjectExists(string id)
        {
            return (_context.Subjects?.Any(e => e.SubjectCode == id)).GetValueOrDefault();
        }
    }
}
