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
			/* var DataSource = await _context.Users
				 .ToListAsync();*/


				var DataSource = from user in _context.Users
											 join studentProject in _context.ClassStudentProjects
											 on user.Id equals studentProject.UserId
											 where studentProject.ProjectId == projectId
												&& studentProject.ClassId == classId
											 select user;
            ViewBag.ProjectId = projectId;
            ViewBag.ClassId = classId;

            DataOperations operation = new DataOperations();
			int count = DataSource.Count();
			return dm.RequiresCounts ? Json(new { result = DataSource, count = count }) : Json(DataSource);
		}


		[HttpPost]
		[Route("/ManageEvaluation/Update")]
		public ActionResult Update([FromBody] CRUDModel<User> value)
		{
			//do stuff
			var ord = value;

			User val = _context.Users.Where(or => or.Id == ord.Value.Id).FirstOrDefault();
			val.Name = ord.Value.Name;
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
