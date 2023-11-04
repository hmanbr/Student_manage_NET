using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Text.Json;

namespace G3.Controllers
{
    public class SubmissionController : Controller
    {
        private readonly SWPContext _context;
        private readonly IFileUploadService _fileUploadService;

        public SubmissionController(SWPContext context, IFileUploadService fileUploadService)
        {
            _context = context;
            this._fileUploadService = fileUploadService;

		}

        [Route("/ManageSubmission/ClassAssignmentList")]
        public async Task<IActionResult> ClassAssignmentList(int classId)
        {
            var ClassAssignmentList = await _context.ClassAssignments
                .Include(ass => ass.Assignment)
                .Where(clsAss => clsAss.ClassId.Equals(classId)).ToListAsync();
            ViewBag.ClassId = classId;
            return View("/Views/Submission/ClassAssignmentList.cshtml", ClassAssignmentList);
        }

        [Route("/ManageSubmission/SubmissionDetail")]
        public async Task<IActionResult> ClassAssignmentDetail(string key)
        {
			//get current logged in user id
			string userJsonString = HttpContext.Session.GetString("User")!;
			int userId = JsonSerializer.Deserialize<User>(userJsonString)!.Id;

			var classAssignmentDetail = await _context.ClassAssignments
                                            .Include(ass => ass.Assignment)
                                            .Include(sub => sub.Submits)
                                            .Where(clsAss => clsAss.Key.Contains(key))
                                            .SingleOrDefaultAsync();

            //Order the submits in assigment detail by Submit time
			classAssignmentDetail.Submits = classAssignmentDetail.Submits
		                                    .OrderByDescending(sub => sub.SubmitTime)
		                                    .ToList();

			var newestSubmission = await _context.Submits
	                                .Where(sub => sub.ClassAssignmentId.Contains(key))
	                                .OrderByDescending(sub => sub.SubmitTime)
	                                .FirstOrDefaultAsync();
			if(newestSubmission == null)
			{
				ViewBag.FileName = "No file submited";
			}else
			{
				string fileName = Path.GetFileName(newestSubmission.FileUrl);
				ViewBag.FileName = fileName;
			}
			
			return View("/Views/Submission/ClassAssignmentDetail.cshtml", classAssignmentDetail);
        }

        [Route("/ManageSubmission/SubmissionCreate")]
        public ActionResult SubmissionCreate(string key, int classId)
        {
            ViewBag.Key = key;
			ViewBag.ClassId = classId;
			return View("/Views/Submission/SubmissionCreate.cshtml");
        }

        [Route("/ManageSubmission/SubmissionCreate")]
        [HttpPost]
		[RequestSizeLimit(100 * 1024 * 1024)]
        public async Task<IActionResult> SubmissionCreate(IFormFile file, string key, int classId)
        {
            if(file != null)
            {
				//get current logged in user id
				string userJsonString = HttpContext.Session.GetString("User")!;
				int userId = JsonSerializer.Deserialize<User>(userJsonString)!.Id;

                var project = await _context.ClassStudentProjects
                                              .Where(project => project.UserId == userId && project.ClassId == classId)
                                              .FirstOrDefaultAsync();

				int projectId = project.ProjectId;

                string folderName = key + "_projectId" + projectId + "_classId" + classId;
				string fileUrl = await _fileUploadService.UploadFileAsync(file, folderName);

				Submit submit = new Submit()
                {
                    FileUrl = fileUrl,
                    ProjectId = projectId,
                    SubmitTime = DateTime.Now,
                    ClassAssignmentId = key
                };
                _context.Add(submit);
                _context.SaveChanges();
				
			}
			ViewBag.Key = key;
			ViewBag.ClassId = classId;
			return RedirectToAction("ClassAssignmentDetail", new { key = key });
		}

		[Route("/ManageSubmission/DownloadFile")]
		public IActionResult DownloadFile(string fileUrl)
		{
			var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", fileUrl);

			if (System.IO.File.Exists(filePath))
			{
				// Determine the content type based on the file extension
				string contentType;
				var extension = Path.GetExtension(fileUrl).ToLower();
				switch (extension)
				{
					case ".pdf":
						contentType = "application/pdf";
						break;
					case ".jpg":
					case ".jpeg":
						contentType = "image/jpeg";
						break;
					case ".png":
						contentType = "image/png";
						break;
					case ".txt":
						contentType = "text/plain";
						break;
					case ".xlsx":
						contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
						break;
					case ".zip":
						contentType = "application/zip";
						break;
					case ".docx":
						contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
						break;
					case ".mp4":
						contentType = "video/mp4";
						break;
					case ".pptx":
						contentType = "application/vnd.openxmlformats-officedocument.presentationml.presentation";
						break;
					default:
						contentType = "application/octet-stream";
						break;
				}

				var originalFileName = Path.GetFileName(fileUrl);
				return File(System.IO.File.ReadAllBytes(filePath), contentType, originalFileName);
			}

			return NotFound();
		}

		/*[HttpGet]
		[Route("/ManageClassroom/DownloadTemplate")]
		public IActionResult DownloadFile()
		{
			string path = "wwwroot/Template/Template.xlsx"; // Use a relative path

			if (System.IO.File.Exists(path))
			{
				var fileBytes = System.IO.File.ReadAllBytes(path);
				return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Template.xlsx");
			}

			return RedirectToAction("UploadExcel");
		}*/

		[Route("/ManageSubmission/SubmissionList")]
		public async Task<IActionResult> SubmissionList(string key)
		{
			var SubmissionList = await _context.Submits
				.Where(sub => sub.ClassAssignmentId.Contains(key)).ToListAsync();


			return View("/Views/Submission/SubmissionList.cshtml", SubmissionList);
		}

		[Route("/ManageSubmission/SubmissionEvaluate")]
        public async Task<IActionResult> SubmissionEvaluate(int submissionId)
        {
			var newestSubmission = await _context.Submits
									.Where(sub => sub.Id.Equals(submissionId))
									.FirstOrDefaultAsync();

			return View("/Views/Submission/SubmissionEvaluate.cshtml", newestSubmission);
        }

        [Route("/ManageSubmission/SubmissionEdit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmissionEdit(int id, IFormCollection collection)
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
