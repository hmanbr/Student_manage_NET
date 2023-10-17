using G3.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace G3.Controllers
{
    public class SubjectSettingsController : Controller
    {
        private readonly SWPContext _context;

        public SubjectSettingsController(SWPContext context)
        {
            _context = context;
        }
        // GET: SubjectSettingsController
        [Route("/SubjectSettings/SubjectSettingsView")]
        public async Task<IActionResult> SubjectSettingsView()
        {
            var SSV = _context.Subjectsettings.Include(s => s.Subject);
            return View(await SSV.ToListAsync());
        }

        // GET: SubjectSettingsController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: SubjectSettingsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SubjectSettingsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: SubjectSettingsController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: SubjectSettingsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: SubjectSettingsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: SubjectSettingsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
