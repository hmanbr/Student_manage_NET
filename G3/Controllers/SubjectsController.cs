using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using G3.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Drawing.Printing;
using MailKit.Search;
using System.Xml.Linq;

namespace G3.Controllers
{
    [AuthActionFilter]
    
    public class SubjectsController : Controller
    {
        private readonly SWPContext _context;

        public SubjectsController(SWPContext context)
        {
            _context = context;
        }

        // GET: Subjects
        [Route("/Subjects/ListSubject")]
        public async Task<IActionResult> SubjectList(int page = 1, int pageSize = 5) {
           

            var query = _context.Subjects.AsQueryable().Include(m => m.Mentor);
            var totalItems = query.Count();

            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            if (page < 1)
            {
               
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

            /*var sWPContext = _context.Subjects.Include(m => m.Mentor);


            return View(await sWPContext.ToListAsync());*/
        }

        [Route("/Subjects/ListSubject")]
        [HttpPost]
        public async Task<IActionResult> SubjectList(string search, string SortBy, string Filter)
        {
            ViewData["search"] = search;
            var Query = from s in _context.Subjects select s; 

            if (!String.IsNullOrEmpty(search))
            {
                Query = Query.Where(x => x.SubjectCode.Contains(search) || x.Name.Contains(search)).Include(m => m.Mentor);

            }

            ViewData["Sort"] = SortBy;
            if (SortBy == "ASC")
            {
                 Query = Query.OrderBy(x => x.SubjectCode).Include(m => m.Mentor);
                
            }
            else
            {
                Query = Query.OrderByDescending(x => x.SubjectCode).Include(m => m.Mentor);
                
            }
           

            return View(await Query.AsNoTracking().ToArrayAsync());

            /*if (SortBy == "ASC")
            {
                var SortQuery = _context.Subjects.OrderBy(x => x.SubjectCode).Include(m => m.Mentor);
                return View(await SortQuery.ToListAsync());
            }
            else if (SortBy == "DESC")
            {
                var SortQuery = _context.Subjects.OrderByDescending(x => x.SubjectCode).Include(m => m.Mentor);
                return View(await SortQuery.ToListAsync());
            }

            if (!String.IsNullOrEmpty(search))
            {
                SearchQuery = SearchQuery.Where(x => x.SubjectCode.Contains(search) || x.Name.Contains(search)).Include(m => m.Mentor);

                return View(await SearchQuery.ToListAsync());
            }
            else
            {
                //var sWPContext = _context.Subjects.Include(m => m.Mentor);
                return await SubjectList();

            }*/
        }
        /*[Route("/Subjects/ListSubject")]
        [HttpPost]
        public async Task<IActionResult> SubjectList(string search)
        {
            ViewData["search"] = search;
            var SearchQuery = from x in _context.Subjects select x;
            
            if (!String.IsNullOrEmpty(search))
            {
                SearchQuery = SearchQuery.Where(x => x.SubjectCode.Contains(search) || x.Name.Contains(search)).Include(m => m.Mentor);
                 
            }
            
            return View(await SearchQuery.ToListAsync());
        }*/

        // GET: Subjects/Details/5
        [Route("/Subjects/Details")]
        public async Task<IActionResult> SubjectDetails(int? id)
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
        public IActionResult SubjectCreate()
        {
            /*var Role = _context.Users.Where(s => s.RoleSettingId == 2);*/
            ViewData["MentorId"] = new SelectList(_context.Users.Where(s => s.RoleSettingId == 2), "Id", "Name");
            return View();
        }

        // POST: Subjects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("/Subjects/Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubjectCreate([Bind("Id,SubjectCode,Name,Status,MentorId")] Subject s)
        {
            if (_context.Subjects.Any(p => p.SubjectCode == s.SubjectCode))
            {
                ViewData["exist"] = "The subject code already exists";
                ViewData["MentorId"] = new SelectList(_context.Users, "Id", "Id");
                
                return View();
               
            }
            _context.Add(s);
                await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Create Successful";
            return RedirectToAction(nameof(SubjectList));
            

        }

        // GET: Subjects/Edit/5
        [Route("/Subjects/Edit")]
        public async Task<IActionResult> SubjectEdit(int? id)
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
            ViewData["MentorId"] = new SelectList(_context.Users.Where(s => s.RoleSettingId == 2), "Id", "Name", subject.MentorId);
            return View(subject);
        }

        // POST: Subjects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("/Subjects/Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubjectEdit(int id, [Bind("Id,SubjectCode,Name,Status,MentorId")] Subject subject)
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
                    TempData["SuccessMessageEdit"] = "Update Successful.";
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
            return RedirectToAction(nameof(SubjectList));
        }

        /*// GET: Subjects/Delete/5
        [Route("/Subjects/Delete")]
        public async Task<IActionResult> SubjectDelete(int? id)
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
        }*/

        [Route("/Subjects/Delete")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Subjects
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            // Xóa sản phẩm và lưu thay đổi vào cơ sở dữ liệu
            
            _context.Subjects.Remove(product);
            await _context.SaveChangesAsync();
           
            return RedirectToAction(nameof(SubjectList));
        }

        private bool SubjectExists(int id)
        {
          return (_context.Subjects?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
