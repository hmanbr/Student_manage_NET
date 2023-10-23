
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace G3.Controllers
{
    public class ClassController : Controller
    {
        private readonly SWPContext _context;

        public ClassController(SWPContext context)
        {
            _context = context;
        }

        // GET: Class
        [Route("/classList")]
        public async Task<IActionResult> Index()
        {

            var sWPContext = _context.Classes.Include(c => c.Subject);
            return View(await sWPContext.ToListAsync());
        }

        // GET: Class/Details/5
        [Route("/classList/{id}")]
        public async Task<IActionResult> Details(int? id, string? tab, [FromServices] IGitLabService gitLabService)
        {
            if (id == null || _context.Classes == null) return NotFound();

            var @class = await _context.Classes.Include(c => c.Subject).FirstOrDefaultAsync(m => m.Id == id);
            if (@class == null) return NotFound();

            ViewData["tab"] = tab ?? "general";

            return tab switch
            {
                "general" => View(),
                "milestones" => RunMilestone(@class, gitLabService),
                _ => View(),
            };

        }

        public IActionResult RunMilestone(Class @class, IGitLabService gitLabService)
        {
            int? groupId = @class.GitLabGroupId;
            if (groupId == null) return View();


            List<NGitLab.Models.Milestone> gitlabMilestone = gitLabService.GetMilestoneByGroupId((int)groupId);
            List<Models.Milestone> milestones = gitlabMilestone.Select(m => new Models.Milestone
            {
                Id = m.Id,
                Iid = m.Iid,
                Title = m.Title,
                Description = m.Description,
                DueDate = DateTime.Now,
                GroupId = m.GroupId,
                StartDate = DateTime.Now,
                State = m.State,
                CreatedAt = m.CreatedAt,
                UpdatedAt = m.UpdatedAt,
                Expired = false,
                WebUrl = "",
            }).ToList();

            string sql = @"
                INSERT INTO `SWP`.`Milestone` (`Id`, `Iid`, `Title`, `Description`, `State`, `CreatedAt`, `UpdatedAt`, `DueDate`, `StartDate`, `Expired`, `WebUrl`, `GroupId`)
                VALUES ";


            for (int i = 0; i < milestones.Count; i++)
            {
                sql += string.Format("({0}, {1}, '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}')",
                    milestones[i].Id, milestones[i].Iid, milestones[i].Title, milestones[i].Description, milestones[i].State,
                    milestones[i].CreatedAt.ToString("yyyy-MM-dd"), milestones[i].UpdatedAt.ToString("yyyy-MM-dd"), milestones[i].DueDate.ToString("yyyy-MM-dd"), milestones[i].StartDate.ToString("yyyy-MM-dd"),
                    milestones[i].Expired ? 1 : 0, "", @class.GitLabGroupId);

                if (i < milestones.Count - 1)
                {
                    sql += ",";
                }
            }

            sql += @"
                ON DUPLICATE KEY UPDATE
                Iid = VALUES(Iid),
                Title = VALUES(Title),
                Description = VALUES(Description),
                State = VALUES(State),
                CreatedAt = VALUES(CreatedAt),
                UpdatedAt = VALUES(UpdatedAt),
                DueDate = VALUES(DueDate),
                StartDate = VALUES(StartDate),
                Expired = VALUES(Expired),
                WebUrl = VALUES(WebUrl),
                GroupId = VALUES(GroupId);
            ";


            _context.Database.ExecuteSqlRaw(sql);

            milestones = _context.Milestones.Where(m => m.GroupId == @class.GitLabGroupId).ToList();
            ViewData["milestones"] = milestones;
            return View();

        }



        // GET: Class/Create
        public IActionResult Create()
        {
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Id");
            return View();
        }

        // POST: Class/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,SubjectId,Status")] Class @class)
        {
            if (ModelState.IsValid)
            {
                _context.Add(@class);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Id", @class.SubjectId);
            return View(@class);
        }

        // GET: Class/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Classes == null)
            {
                return NotFound();
            }

            var @class = await _context.Classes.FindAsync(id);
            if (@class == null)
            {
                return NotFound();
            }
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Id", @class.SubjectId);
            return View(@class);
        }

        // POST: Class/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,SubjectId,Status")] Class @class)
        {
            if (id != @class.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@class);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClassExists(@class.Id))
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
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Id", @class.SubjectId);
            return View(@class);
        }

        // GET: Class/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Classes == null)
            {
                return NotFound();
            }

            var @class = await _context.Classes
                .Include(c => c.Subject)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@class == null)
            {
                return NotFound();
            }

            return View(@class);
        }

        // POST: Class/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Classes == null)
            {
                return Problem("Entity set 'SWPContext.Classes'  is null.");
            }
            var @class = await _context.Classes.FindAsync(id);
            if (@class != null)
            {
                _context.Classes.Remove(@class);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClassExists(int id)
        {
            return (_context.Classes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
