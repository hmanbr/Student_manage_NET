using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace G3.Controllers
{
    public class MilestoneController : Controller
    {
        private readonly SWPContext _context;

        public MilestoneController(SWPContext context)
        {
            _context = context;
        }

        [Route("/classes/:classId")]
        public async Task<IActionResult> Index(int classId, [FromQuery] string tabName)
        {
            // general

            // milestone
            var milestones = _context.Milestones.Where(milestone => milestone.ClassId == classId);

            // setting

            // student list
            return View(await milestones.ToListAsync());
        }


        // GET: Milestone/Details/5
        [Route("/milestone/:id")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Milestones == null)
            {
                return NotFound();
            }

            var milestone = await _context.Milestones
                .Include(m => m.Class)
                .Include(m => m.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (milestone == null)
            {
                return NotFound();
            }

            return View(milestone);
        }

        // GET: Milestone/Create
        public IActionResult Create()
        {
            ViewData["ClassId"] = new SelectList(_context.Classes, "Id", "Id");
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Id");
            return View();
        }

        // POST: Milestone/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Iid,ProjectId,Title,Description,State,CreatedAt,UpdatedAt,DueDate,StartDate,Expired,WebUrl,ClassId")] Milestone milestone)
        {
            if (ModelState.IsValid)
            {
                _context.Add(milestone);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClassId"] = new SelectList(_context.Classes, "Id", "Id", milestone.ClassId);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Id", milestone.ProjectId);
            return View(milestone);
        }

        // GET: Milestone/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Milestones == null)
            {
                return NotFound();
            }

            var milestone = await _context.Milestones.FindAsync(id);
            if (milestone == null)
            {
                return NotFound();
            }
            ViewData["ClassId"] = new SelectList(_context.Classes, "Id", "Id", milestone.ClassId);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Id", milestone.ProjectId);
            return View(milestone);
        }

        // POST: Milestone/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Iid,ProjectId,Title,Description,State,CreatedAt,UpdatedAt,DueDate,StartDate,Expired,WebUrl,ClassId")] Milestone milestone)
        {
            if (id != milestone.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(milestone);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MilestoneExists(milestone.Id))
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
            ViewData["ClassId"] = new SelectList(_context.Classes, "Id", "Id", milestone.ClassId);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Id", milestone.ProjectId);
            return View(milestone);
        }

        // GET: Milestone/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Milestones == null)
            {
                return NotFound();
            }

            var milestone = await _context.Milestones
                .Include(m => m.Class)
                .Include(m => m.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (milestone == null)
            {
                return NotFound();
            }

            return View(milestone);
        }

        // POST: Milestone/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Milestones == null)
            {
                return Problem("Entity set 'SWPContext.Milestones'  is null.");
            }
            var milestone = await _context.Milestones.FindAsync(id);
            if (milestone != null)
            {
                _context.Milestones.Remove(milestone);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MilestoneExists(int id)
        {
            return (_context.Milestones?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
