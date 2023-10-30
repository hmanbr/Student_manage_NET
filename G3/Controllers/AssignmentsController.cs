using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using G3.Models;
using MySqlX.XDevAPI.Common;
using DocumentFormat.OpenXml.Wordprocessing;

namespace G3.Controllers
{
    public class AssignmentsController : Controller
    {
        private readonly SWPContext _context;

        public AssignmentsController(SWPContext context)
        {
            _context = context;
        }

        // GET: Assignments
        [Route("/subjectAssignment")]
        public async Task<IActionResult> SubAsmList(string sortOrder, string searchString, string selectFilter, int page = 1, int pageSize = 8)
        {
            ViewData["NameSort"] = String.IsNullOrEmpty(sortOrder) ? "nameDesc" : "";
            ViewData["DateSort"] = sortOrder == "title" ? "titleDesc" : "title";
            ViewData["SearchAss"] = searchString;
   
            var assign = from s in _context.Assignments.Include(a => a.Subject) select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                assign = assign.Where(s => 
                s.Title.Contains(searchString) || 
                s.Subject.Name.Contains(searchString) || 
                s.Subject.SubjectCode.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "nameDesc":
                    assign = assign.OrderByDescending(s => s.Subject.Name);
                    break;
                case "title":
                    assign = assign.OrderByDescending(s => s.Title);
                    break;
                case "titleDesc":
                    assign = assign.OrderBy(s => s.Title);
                    break;
                default:
                    assign = assign.OrderBy(s => s.Subject.Name);
                    break;
            }
           
            /*var sWPContext = _context.Assignments.Include(a => a.Subject);*/
            var totalItems = assign.Count();

            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            if (page < 1)
            {
                page = 1;
            }
            else if (page > totalPages)
            {
                page = totalPages;
            }
            assign = assign
                .Skip((page - 1) * pageSize)
                .Take(pageSize).Include(m => m.Subject);
            ViewBag.TotalItems = totalItems;
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;

            return View(await assign.ToListAsync());
        }

        // GET: Assignments/Details/5
        [Route("/assignmentDetails")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Assignments == null)
            {
                return NotFound();
            }

            var assignment = await _context.Assignments
                .Include(a => a.Subject)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (assignment == null)
            {
                return NotFound();
            }

            return View(assignment);
        }

        // GET: Assignments/Create
        [Route("/assignmentCreate")]
        public IActionResult Create()
        {
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "SubjectCode");
            return View();
        }

        // POST: Assignments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/assignmentCreate")]
         public async Task<IActionResult> Create([Bind("Id,Title,Description,SubjectId")] Assignment assignment)
        {
            if (!ModelState.IsValid)
            {
                _context.Add(assignment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(SubAsmList));
            }
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Id", assignment.SubjectId);
            return View(assignment);
        }


        // GET: Assignments/Edit/5
        [Route("/assignmentEdit")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Assignments == null)
            {
                return NotFound();
            }

            var assignment = await _context.Assignments.FindAsync(id);
            if (assignment == null)
            {
                return NotFound();
            }
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "SubjectCode", assignment.SubjectId);
            return View(assignment);
        }

        // POST: Assignments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/assignmentEdit")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,SubjectId")] Assignment assignment)
        {
            if (id != assignment.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                try
                {
                    _context.Update(assignment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AssignmentExists(assignment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(SubAsmList));
            }
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Id", assignment.SubjectId);
            return View(assignment);
        }


        // GET: Assignments/Delete/5
        [Route("/assignmentDelete")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Assignments == null)
            {
                return NotFound();
            }

            var assignment = await _context.Assignments
                .Include(a => a.Subject)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (assignment == null)
            {
                return NotFound();
            }

            return View(assignment);
        }

        // POST: Assignments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("/assignmentDelete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Assignments == null)
            {
                return Problem("Entity set 'SWPContext.Assignments'  is null.");
            }
            var assignment = await _context.Assignments.FindAsync(id);
            if (assignment != null)
            {
                _context.Assignments.Remove(assignment);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(SubAsmList));
        }

        private bool AssignmentExists(int id)
        {
          return (_context.Assignments?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
