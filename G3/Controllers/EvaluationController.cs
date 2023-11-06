using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Syncfusion.EJ2.Base;

namespace G3.Controllers
{
    public class EvaluationController : Controller
    {
        private readonly SWPContext _context;
        public EvaluationController(SWPContext context)
        {
            _context = context;
        }

        [Route("/ManageEvaluation/UserList")]
        public async Task<IActionResult> Index(int projectId, int classId)
        {
            ViewBag.ProjectId = projectId;
            ViewBag.ClassId = classId;
            return View("/Views/Evaluation/Index.cshtml");
        }

        [HttpPost]
        [Route("/ManageEvaluation/UrlDataSource")]
        public async Task<IActionResult> UrlDataSource([FromBody] DataManagerRequest dm, int projectId, int classId)
        {
			/*var DataSource = await _context.Projects
                .ToListAsync();*/

			var DataSource = _context.ClassStudentProjects
	                    .Where(csp => csp.ProjectId == projectId && csp.ClassId == classId)
	                    .Select(csp => csp.User)
	                    .ToList();
			DataOperations operation = new DataOperations();
            int count = DataSource.Count();
            return dm.RequiresCounts ? Json(new { result = DataSource, count = count }) : Json(DataSource);
        }


		[HttpPost]
		[Route("/ManageEvaluation/Update")]
		public ActionResult Update([FromBody] CRUDModel<Submit> value)
		{
			//do stuff
			var ord = value;

			Submit val = _context.Submits.Where(or => or.Id == ord.Value.Id).FirstOrDefault();
			val.FileUrl = ord.Value.FileUrl;
			_context.SaveChanges();
			return Json(value);
		}

		// POST: EvaluationController/Edit/5
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

        // GET: EvaluationController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: EvaluationController/Delete/5
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
