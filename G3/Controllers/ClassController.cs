
using DocumentFormat.OpenXml.Office2010.Excel;
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
        public async Task<IActionResult> Index(string sortOrder, string searchString, string selectFilter, int page = 1, int pageSize = 5)
        {
            ViewData["NameSort"] = String.IsNullOrEmpty(sortOrder) ? "nameDesc" : "";
            ViewData["StatusSort"] = sortOrder == "title" ? "titleDesc" : "title";
            ViewData["SearchAss"] = searchString;

            var assign = from s in _context.Classes.Include(a => a.Subject) select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                assign = assign.Where(s =>
                s.Name.Contains(searchString) ||
                s.Subject.Name.Contains(searchString) ||
                s.Subject.SubjectCode.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "nameDesc":
                    assign = assign.OrderByDescending(s => s.Name);
                    break;
                case "title":
                    assign = assign.OrderByDescending(s => s.Status);
                    break;
                case "titleDesc":
                    assign = assign.OrderBy(s => s.Status);
                    break;
                default:
                    assign = assign.OrderBy(s => s.Name);
                    break;
            }

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

        // GET: Class/Details/5
        [Route("/classList/{id}")]
        public async Task<IActionResult> Details(int? id, string? tab, [FromServices] IGitLabClient gitLabClient)
        {
            if (id == null || _context.Classes == null) return NotFound();

            var @class = await _context.Classes.Include(c => c.Subject).FirstOrDefaultAsync(m => m.Id == id);
            if (@class == null) return NotFound();

            ViewData["tab"] = tab ?? "general";
            ViewData["class"] = @class;

            return tab switch
            {
                "general" => View(),
                "milestones" => RunMilestone(@class, gitLabClient),
                _ => View(),
            };

        }

        public IActionResult RunMilestone(Class @class, IGitLabClient gitLabClient)
        {
            int? groupId = @class.GitLabGroupId;
            if (groupId == null) return View();


            List<NGitLab.Models.Milestone> gitlabMilestone = gitLabClient.GetGroupMilestone((int)groupId).All.ToList();
            List<Models.Milestone> milestones = gitlabMilestone.Select(m => new Models.Milestone
            {
                Id = m.Id,
                Iid = m.Iid,
                Title = m.Title,
                Description = m.Description,
                DueDate = DateTime.Parse(m.DueDate),
                GroupId = m.GroupId,
                StartDate = DateTime.Parse(m.StartDate),
                State = m.State,
                CreatedAt = m.CreatedAt,
                UpdatedAt = m.UpdatedAt,
            }).ToList();

            string sql = @"
                INSERT INTO `SWP`.`Milestone` (`Id`, `Iid`, `Title`, `Description`, `State`, `CreatedAt`, `UpdatedAt`, `DueDate`, `StartDate`, `GroupId`)
                VALUES ";


            for (int i = 0; i < milestones.Count; i++)
            {
                sql += string.Format("({0}, {1}, '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}')",
                    milestones[i].Id, milestones[i].Iid, milestones[i].Title, milestones[i].Description, milestones[i].State,
                    milestones[i].CreatedAt.ToString("yyyy-MM-dd"), milestones[i].UpdatedAt.ToString("yyyy-MM-dd"), milestones[i].DueDate.ToString("yyyy-MM-dd"), milestones[i].StartDate.ToString("yyyy-MM-dd"),
                    @class.GitLabGroupId);

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
                GroupId = VALUES(GroupId);
            ";


            _context.Database.ExecuteSqlRaw(sql);

            milestones = _context.Milestones.Where(m => m.GroupId == @class.GitLabGroupId).ToList();
            ViewData["milestones"] = milestones;
            return View();
        }

        [Route("/classList/{id}")]
        [HttpPost]
        public async Task<ActionResult> CreateMilestone([Bind("Title, Description, StartDate, DueDate")] MilestoneDto milestoneDto, string tab, [FromServices] IGitLabClient gitLabClient, int? id)
        {

            if (id == null || _context.Classes == null) return NotFound();

            var @class = await _context.Classes.Include(c => c.Subject).FirstOrDefaultAsync(m => m.Id == id);
            if (@class == null) return NotFound();
            int? groupId = @class.GitLabGroupId;
            if (groupId == null) return View();


            NGitLab.Models.Milestone milestoneCreate = gitLabClient.GetGroupMilestone((int)groupId).Create(new NGitLab.Models.MilestoneCreate
            {
                Title = milestoneDto.Title,
                Description = milestoneDto.Description,
                StartDate = milestoneDto.StartDate,
                DueDate = milestoneDto.DueDate
            });

            Milestone milestone = new Milestone
            {
                Id = milestoneCreate.Id,
                Iid= milestoneCreate.Iid,
                Title =milestoneCreate.Title,
                Description = milestoneCreate.Description,
                StartDate = DateTime.Parse( milestoneCreate.StartDate),
                DueDate = DateTime.Parse(milestoneCreate.DueDate),
                GroupId = groupId,
                State = milestoneCreate.State
               
            };
            _context.Milestones.Add(milestone);
            await _context.SaveChangesAsync();

            return Redirect("/classList/" + id + "?tab=milestones");
        }

        [Route("milestones/{id}")]
        public async Task<IActionResult> MilestoneDetail(int? id)
        {

            if (id == null || _context.Milestones == null) return NotFound();

            var Milestone = await _context.Milestones.Include(m => m.Group).FirstOrDefaultAsync(m => m.Id == id);
            if (Milestone == null) return NotFound();

            ViewData["Milestone"] = Milestone;
            return View();
        }



        // GET: Class/Create
        [Route("/classNew")]
        public IActionResult Create()
        {
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "SubjectCode");
            return View();
        }

        // POST: Class/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/classNew")]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,GitLabGroupId,SubjectId,Status")] Class @class)
        {

            if (ModelState.IsValid)
            {

                if (_context.Classes.Any(c => c.Name == @class.Name))
                {
                    ViewData["error"] = "This class has exist!!";
                }
                else
                {
                    _context.Add(@class);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }


            }
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Id", @class.SubjectId);
            return View(@class);
        }

        // GET: Class/Edit/5
        [Route("/classEdit")]
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
        [Route("/classEdit")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,GitLabGroupId,SubjectId,Status")] Class @class)
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
        [Route("/classDelete")]
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
        [Route("/classDelete")]
        public async Task<IActionResult> Delete(int id)
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

        [Route("/admin/editClass_toggle2")]
        public IActionResult Edit_toggle2(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var settingFromDB = _context.Classes.Find(id);
            if (settingFromDB == null)
            {
                return NotFound();
            }
            settingFromDB.Status = !settingFromDB.Status;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
