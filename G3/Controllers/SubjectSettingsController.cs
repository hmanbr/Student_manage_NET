using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using G3.Models;
using DocumentFormat.OpenXml.VariantTypes;

namespace G3.Controllers
{
    public class SubjectSettingsController : Controller
    {
        private readonly SWPContext _context;

        public SubjectSettingsController(SWPContext context)
        {
            _context = context;
        }

        // GET: SubjectSettings
        [Route("/SubjectSettings/SubjectSettingList")]
        public async Task<IActionResult> SubjectSettingList(int? ClassId, string search, string SortBy, string StatusFilter, int page = 1, int pageSize = 5)
        {
            var query = _context.SubjectSettings.Include(x => x.Subject);
            ViewData["search"] = search;
            if (!String.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.Subject.SubjectCode.Contains(search) || x.Type.Contains(search)).Include(x => x.Subject);
            }

            ViewData["Sort"] = SortBy;
            if (SortBy == "ASC")
            {
                query = query.OrderBy(x => x.Subject.SubjectCode).Include(x => x.Subject);
            }
            else if (SortBy == "DESC")
            {
                query = query.OrderByDescending(x => x.Subject.SubjectCode).Include(x => x.Subject);
            }

            if (StatusFilter == "true")
            {
                query = query.Where(x => x.Status == true).Include(x => x.Subject);

            }
            else if (StatusFilter == "false")
            {
                query = query.Where(x => x.Status == false).Include(x => x.Subject);
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
                .Take(pageSize).Include(x => x.Subject);
            ViewBag.TotalItems = totalItems;
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;

            /*ClassId = ClassId ?? 0;
            var classIdd = _context.SubjectSettings.ToList();
            classIdd.Insert(0, new SubjectSetting{ Id = 0, Type = "All" });

            ViewBag.ClassId = new SelectList(classIdd.Select(x=>x.Type.Distinct()), "Id", "Type", ClassId);

            if (query.Any(x => x.Id == ClassId))
            {
                query = query.Where(x => x.Id == ClassId).Include(x => x.Subject);
            }*/
            return View(await query.ToListAsync());
        }

        [Route("/SubjectSettings/SubjectSettingCreate")]
        // GET: SubjectSettings/Create
        public IActionResult SubjectSettingCreate()
        {
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "SubjectCode");
            return View();
        }

        // POST: SubjectSettings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("/SubjectSettings/SubjectSettingCreate")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubjectSettingCreate([Bind("Id,Type,Value,Status,Name,SubjectId")] SubjectSetting subjectSetting)
        {
            if (_context.SubjectSettings.Any(x => x.Type == subjectSetting.Type &&  x.Name == subjectSetting.Name && x.Value == subjectSetting.Value && x.SubjectId == subjectSetting.SubjectId))
            {
                ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "SubjectCode", subjectSetting.SubjectId);
                ViewData["exist"] = "data cannot be the same";
                return View(subjectSetting);

            }

            _context.Add(subjectSetting);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Create Successful";
            return RedirectToAction(nameof(SubjectSettingList));

        }
        [Route("/SubjectSettings/SubjectSettingEdit")]
        // GET: SubjectSettings/Edit/5
        public async Task<IActionResult> SubjectSettingEdit(int? id)
        {
            if (id == null || _context.SubjectSettings == null)
            {
                return NotFound();
            }

            var subjectSetting = await _context.SubjectSettings.FindAsync(id);
            if (subjectSetting == null)
            {
                return NotFound();
            }
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "SubjectCode", subjectSetting.SubjectId);
            return View(subjectSetting);
        }

        // POST: SubjectSettings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("/SubjectSettings/SubjectSettingEdit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubjectSettingEdit(int id, [Bind("Id,Type,Value,Status,Name,SubjectId")] SubjectSetting subjectSetting)
        {
            if (id != subjectSetting.Id)
            {
                return NotFound();
            }

            if (_context.SubjectSettings.Any(x => x.Type == subjectSetting.Type && x.Name == subjectSetting.Name && x.Value == subjectSetting.Value && x.SubjectId == subjectSetting.SubjectId && x.Id != id))
            {
                ViewData["exist"] = "data cannot be the same";
                ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "SubjectCode", subjectSetting.SubjectId);
                return View(subjectSetting);
            }
            else { 
                try
                {
                    _context.Update(subjectSetting);
                    TempData["SuccessMessageEdit"] = "Update Successful.";
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubjectSettingExists(subjectSetting.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                
            }
            return RedirectToAction(nameof(SubjectSettingList));

        }

        [Route("/SubjectSettings/Edit_toggle")]
        public IActionResult Edit_toggle(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var settingFromDB = _context.SubjectSettings.Find(id);
            if (settingFromDB == null)
            {
                return NotFound();
            }
            settingFromDB.Status = !settingFromDB.Status;
            _context.SaveChanges();
            return RedirectToAction("SubjectSettingList");
        }

        private bool SubjectSettingExists(int id)
        {
          return (_context.SubjectSettings?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
