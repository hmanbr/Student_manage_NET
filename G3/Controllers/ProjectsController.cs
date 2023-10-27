using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using G3.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace G3.Controllers
{
    [AuthActionFilter]
    public class ProjectsController : Controller
    {
        private readonly SWPContext _context;

        public ProjectsController(SWPContext context)
        {
            _context = context;
        }

        // GET: Projects
        [Route("/Projects/ProjectList")]
        public async Task<IActionResult> ProjectList( int? ClassId, int? MentorID, string search, string SortBy,string StatusFilter, string ClassFilter, int page = 1, int pageSize = 5)
        {


            var query = _context.Projects.Include(p => p.Class).Include(p => p.Mentor);


            ViewData["search"] = search;
            if (!String.IsNullOrEmpty(search))
            {
                query =  query.Where(x => x.ProjectCode.Contains(search) || x.EnglishName.Contains(search)).Include(p => p.Class).Include(p => p.Mentor);
            }

            ViewData["Sort"] = SortBy;
            if (SortBy == "ASC")
            {
                query = query.OrderBy(x => x.ProjectCode).Include(p => p.Class).Include(p => p.Mentor);
            }
            else if(SortBy =="DESC")
            {
                query = query.OrderByDescending(x => x.ProjectCode).Include(p => p.Class).Include(p => p.Mentor);
            }

            if (StatusFilter == "true" )
            {              
                query = query.Where(x => x.Status == true).Include(p => p.Class).Include(p => p.Mentor);
                
            }
            else if (StatusFilter == "false")
            {
                query = query.Where(x => x.Status == false).Include(p => p.Class).Include(p => p.Mentor);
            }

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

            query = query

                .Skip((page - 1) * pageSize)
                .Take(pageSize).Include(p => p.Class).Include(p => p.Mentor);
            ViewBag.TotalItems = totalItems;
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;

            
            ClassId = ClassId ?? 0;
            var classIdd = _context.Classes.ToList();
            classIdd.Insert(0, new Class { Id = 0, Name = "All" });

            ViewBag.ClassId = new SelectList(classIdd, "GitLabGroupId", "Name", ClassId);

            if (query.Any(x=>x.ClassId==ClassId))
            {
                query = query.Where(x => x.ClassId == ClassId).Include(p => p.Class).Include(p => p.Mentor);
            }

            /*MentorID = MentorID ?? 0;
            var MentorIDD = _context.Users.ToList();
            MentorIDD.Insert(0, new User { Id = 0, Name = "All" });

            ViewBag.MentorId = new SelectList(MentorIDD.Where(x =>x.RoleSettingId == 4), "Id", "Name", MentorID);

            if (query.Any(x => x.MentorId == MentorID ))
            {
                query = query.Where(x => x.MentorId == MentorID).Include(p => p.Class).Include(p => p.Mentor);
            }*/

            return View(await query.ToListAsync());
        }

        
       

        [Route("/Projects/ProjectNew")]
        // GET: Projects/Create
        public IActionResult ProjectNew()
        {
            ViewData["ClassId"] = new SelectList(_context.Classes, "GitLabGroupId", "Name");
            ViewData["MentorId"] = new SelectList(_context.Users.Where(s => s.RoleSettingId == 4), "Id", "Name");
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("/Projects/ProjectNew")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProjectNew([Bind("Id,ProjectCode,EnglishName,VietNameseName,Status,Description,GroupName,MentorId,ClassId")] Project project)
        {
            if (_context.Projects.Any(p => p.ProjectCode == project.ProjectCode))
            {
                ViewData["exist"] = "The Project code already exists";
                ViewData["ClassId"] = new SelectList(_context.Classes, "Id", "Name", project.ClassId);
                ViewData["MentorId"] = new SelectList(_context.Users, "Id", "Name", project.MentorId);
                return View(project);
               
            }
            _context.Add(project);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Create Successful";
            return RedirectToAction(nameof(ProjectList));

        }

        [Route("/Projects/ProjectDetail")]
        // GET: Projects/Edit/5
        public async Task<IActionResult> ProjectDetail(int? id)
        {
            if (id == null || _context.Projects == null)
            {
                return NotFound();
            }
            
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            ViewData["ClassId"] = new SelectList(_context.Classes, "GitLabGroupId", "Name", project.ClassId);
            ViewData["MentorId"] = new SelectList(_context.Users.Where(u => u.RoleSettingId==4), "Id", "Name", project.MentorId);        
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("/Projects/ProjectDetail")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProjectDetail(int id, [Bind("Id,ProjectCode,EnglishName,VietNameseName,Status,Description,GroupName,MentorId,ClassId")] Project project)
        {
            if (id != project.Id)
            {
                return NotFound();
            }

            if (_context.Projects.Any(p => p.ProjectCode == project.ProjectCode && p.Id != id))
            {
                ViewData["exist"] = "The Project code already exists";
                ViewData["ClassId"] = new SelectList(_context.Classes, "Id", "Name", project.ClassId);
                ViewData["MentorId"] = new SelectList(_context.Users, "Id", "Name", project.MentorId);
                return View(project);

            }
            else
            {
                try
                {                                    
                    _context.Update(project);
                    TempData["SuccessMessageEdit"] = "Update Successful.";
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
               
            }
            return RedirectToAction(nameof(ProjectList));
        }



        /*   [Route("/Projects/ProjectDelete")]
           public async Task<IActionResult> ProjectDelete(int? id)
           {
               if (id == null)
               {
                   return NotFound();
               }
               var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == id);
               if (project == null)
               {
                   return NotFound();
               }

               if (project.Status == true)
               {

                   TempData["DeleteMessageFales"] = "Delete False, you can not delete an active project";
                   return RedirectToAction(nameof(ProjectList));

               }

               _context.Projects.Remove(project);
               TempData["DeleteMessageSuccess"] = "Delete Successful";
               await _context.SaveChangesAsync();
               return RedirectToAction(nameof(ProjectList));

           }*/

        [Route("/Projects/ProjectDelete")]
        // GET: Projects/Delete/5
        public async Task<IActionResult> ProjectDelete(int? id)
        {
            if (id == null || _context.Projects == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .Include(p => p.Class)
                .Include(p => p.Mentor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }
        // POST: Projects/Delete/5
        [Route("/Projects/ProjectDelete")]
        [HttpPost, ActionName("ProjectDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProjectDeleteConfirmed(int id)
        {
            if (_context.Projects == null)
            {
                return Problem("Entity set 'SWPContext.Projects'  is null.");
            }
            var project = await _context.Projects.Include(p => p.Class)
                .Include(p => p.Mentor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            if (project.Status == true)
            {

                TempData["DeleteMessageFales"] = "Delete False, you can not delete an active project";

                return View(project);

            }
            _context.Projects.Remove(project);
            TempData["DeleteMessageSuccess"] = "Delete Successful";
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ProjectList));

        }

        private bool ProjectExists(int id)
        {
          return (_context.Projects?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
