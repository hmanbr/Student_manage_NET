using ClosedXML.Excel;
using ExcelDataReader;
using G3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Data;
using System.Text;
using G3.Views.Shared.Components.SearchBar;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using G3.Services;
using MailKit;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace G3.Controllers
{
	public class ClassroomController : Controller
	{
		private readonly SWPContext _context;
		private readonly IWebHostEnvironment _webHostEnvironment;

		public ClassroomController(SWPContext context, IWebHostEnvironment webHostEnvironment)
		{
			_context = context;
		}

		[Route("/ManageClassroom/ClassesList")]
		public async Task<IActionResult> ClassList(string search, int pg = 1)
		{
			var classlist = await _context.Classes.Include(subject => subject.Subject).ToListAsync();

			if (!string.IsNullOrEmpty(search))
			{
				classlist = await _context.Classes
					.Where(cls => cls.Name.Contains(search))
					.Include(subject => subject.Subject).ToListAsync();
			}

			const int pageSize = 5;
			if (pg < 1)
			{
				pg = 1;
			}

			int recsCount = classlist.Count();

			var pager = new Pager(recsCount, pg, pageSize);

			int recSkip = (pg - 1) * pageSize;

			var data = classlist.Skip(recSkip).Take(pager.PageSize).ToList();

			SPager searchPager = new SPager(recsCount, pg, pageSize)
			{
				Action = "ClassList",
				Controller = "Classroom",
				SearchText = search,
			};

			this.ViewBag.Pager = pager;

			ViewBag.SearchString = search;
			ViewBag.SearchPager = searchPager;

			// Pass the list of settings to the view.
			return View("/Views/Classroom/ClassesList.cshtml", data);
		}

		[Route("/ManageClassroom/ClassStudents")]
		public async Task<IActionResult> ClassStudents(int classId, string search, int pg = 1)
		{
			var usersInClass = await _context.ClassStudentProjects
								.Where(csp => csp.ClassId == classId) // Filter by the class Id
								.Include(csp => csp.User) // Eager loading to load associated users
								.Select(csp => csp.User)
								.Where(user => user.RoleSettingId == 5)
								.ToListAsync();

			if (!string.IsNullOrEmpty(search))
			{
				usersInClass = await _context.ClassStudentProjects
								.Where(csp => csp.ClassId == classId) // Filter by the class Id
								.Include(csp => csp.User) // Eager loading to load associated users
								.Select(csp => csp.User)
								.Where(user => user.RoleSettingId == 5) // Filter by RoleSettingId
								.Where(user => user.Name.Contains(search)) // Search by user name
								.ToListAsync();
			}

			const int pageSize = 5;
			if (pg < 1)
			{
				pg = 1;
			}

			int recsCount = usersInClass.Count();

			var pager = new Pager(recsCount, pg, pageSize);

			int recSkip = (pg - 1) * pageSize;

			var data = usersInClass.Skip(recSkip).Take(pager.PageSize).ToList();

			SPager searchPager = new SPager(recsCount, pg, pageSize)
			{
				Action = "ClassStudents",
				Controller = "Classroom",
				SearchText = search,
			};

			this.ViewBag.Pager = pager;

			ViewBag.SearchString = search;
			ViewBag.SearchPager = searchPager;
			ViewBag.ClassId = classId;
			return View("/Views/Classroom/ClassStudents.cshtml", data);
		}

		[Route("/ManageClassroom/ClassDetail")]
		public async Task<IActionResult> ClassDetail(int id)
		{
			/*var usersInClass = await _context.Classes
							.Where(cls => cls.Id == id)
							.Include(cls => cls.Users) // Eager loading to load associated users
							.SelectMany(cls => cls.Users)
							.Take(5) // Limit to the first 5 users
							.ToListAsync();*/

			var usersInClass = await _context.ClassStudentProjects
								.Where(csp => csp.ClassId == id) // Filter by the class Id
								.Include(csp => csp.User) // Eager loading to load associated users
								.Select(csp => csp.User)
								.Take(5) // Limit to the first 5 users
								.ToListAsync();


			ViewBag.ClassId = id;
			return View("/Views/Classroom/ClassDetail.cshtml", usersInClass);
		}



		[Route("/ManageClassroom/ClassAddStudents")]
		public async Task<IActionResult> ClassAddStudents(int id)
		{
			var studentList = await _context.Users.Where(userList => userList.RoleSettingId == 5).ToListAsync();
			ViewBag.ClassId = id;
			return View("/Views/Classroom/ClassAddStudents.cshtml", studentList);
		}

		[Route("ManageClassroom/AddStudentsToClass")]
		[HttpPost]
		public async Task<IActionResult> AddStudentsToClass(int stuId, int classId)
		{
			User existingUser = _context.Users.FirstOrDefault(u => u.Id == stuId);

			var classToAddUserTo = await _context.ClassStudentProjects
							.Where(csp => csp.ClassId == classId)
							.FirstOrDefaultAsync();

			if (classToAddUserTo != null)
			{
				// Check if the user is already associated with the class
				var isUserAlreadyInClass = await _context.ClassStudentProjects
					.AnyAsync(csp => csp.ClassId == classId && csp.UserId == existingUser.Id);

				if (!isUserAlreadyInClass)
				{
					// Create a new ClassStudentProject entry for the user
					var newClassStudentProject = new ClassStudentProject
					{
						UserId = existingUser.Id,
						ClassId = classToAddUserTo.ClassId,
						ProjectId = 1,
						Status = true,

						// Set the ProjectId property if applicable
					};

					_context.ClassStudentProjects.Add(newClassStudentProject);
					await _context.SaveChangesAsync(); // Save the new class-user relationship
				}
			}
			var usersInClass = await _context.ClassStudentProjects
								.Where(csp => csp.ClassId == classId) // Filter by the class Id
								.Include(csp => csp.User) // Eager loading to load associated users
								.Select(csp => csp.User)
								.Take(5) // Limit to the first 5 users
								.ToListAsync();
			int pg = 1;
			const int pageSize = 5;
			if (pg < 1)
			{
				pg = 1;
			}

			int recsCount = usersInClass.Count();

			var pager = new Pager(recsCount, pg, pageSize);

			int recSkip = (pg - 1) * pageSize;

			var data = usersInClass.Skip(recSkip).Take(pager.PageSize).ToList();

			SPager searchPager = new SPager(recsCount, pg, pageSize)
			{
				Action = "ClassStudents",
				Controller = "Classroom",
				SearchText = string.Empty,
			};

			this.ViewBag.Pager = pager;

			ViewBag.SearchString = string.Empty;
			ViewBag.SearchPager = searchPager;
			ViewBag.ClassId = classId;
			return RedirectToAction("ClassStudents", "Classroom", new { classId = classId });
		}

		[Route("/ManageClassroom/UploadExcel")]
		public async Task<IActionResult> UploadExcel(int id)
		{
			ViewBag.ClassId = id;
			return View("/Views/Classroom/UploadExcel.cshtml");
		}

		[Route("/ManageClassroom/UploadExcel")]
		[HttpPost]
		public async Task<IActionResult> UploadExcel(IFormFile file, int id, [FromServices] IHashService hashService, [FromServices] Services.IMailService mailService) // need to fix this excel function
		{
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

			if (file != null && file.Length > 0)
			{
				var uploadFolder = $"{Directory.GetCurrentDirectory()}\\wwwroot\\Uploads";
				if (!Directory.Exists(uploadFolder))
				{
					Directory.CreateDirectory(uploadFolder);
				}
				var filePath = Path.Combine(uploadFolder, file.Name);

				using (var stream = new FileStream(filePath, FileMode.Create))
				{
					await file.CopyToAsync(stream);
				}

				using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
				{
					// Auto-detect format, supports:
					//  - Binary Excel files (2.0-2003 format; *.xls)
					//  - OpenXml Excel files (2007 format; *.xlsx, *.xlsb)
					using (var reader = ExcelReaderFactory.CreateReader(stream))
					{
						bool wrongFormat = false;
						do
						{
							bool isHeaderSkipped = false;
							while (reader.Read())
							{
								if (!isHeaderSkipped)
								{
									isHeaderSkipped = true;
									continue;
								}
								User user = new User();

								object emailValueCheck = reader.GetValue(0);
								if (emailValueCheck == null)
								{
									ViewBag.Message = "wrongFormat";
									wrongFormat = true;
									break;
								}
								user.Email = reader.GetValue(0).ToString();

								if (user.Email.Contains("gmail.com"))
								{
									user.DomainSettingId = 7;
								}
								else if (user.Email.Contains("fpt.edu.vn"))
								{
									user.DomainSettingId = 6;
								}

								object nameValueCheck = reader.GetValue(1);
								if (nameValueCheck == null)
								{
									ViewBag.Message = "wrongFormat";
									wrongFormat = true;
									break;
								}
								user.Name = reader.GetValue(1).ToString();
								user.RoleSettingId = 5;
								user.Status = true;
								bool classStatus;
								if (reader.GetValue(2).ToString().Equals("1"))
								{
									classStatus = true;
								}
								else
								{
									classStatus = false;
								}


								string emailToCheck = user.Email;

								// Check if the user already exists in the database
								User existingUser = _context.Users.FirstOrDefault(u => u.Email == emailToCheck);

								if (existingUser == null)
								{
									String randomPass = hashService.RandomStringGenerator(8);
									user.Hash = hashService.HashPassword(randomPass);
									mailService.SendPassword(user.Email, randomPass);
									_context.Add(user);
									await _context.SaveChangesAsync(); // Save the new user
									existingUser = user; // Update existingUser with the newly added user
								}



								int classId = id;

								var classToAddUserTo = await _context.ClassStudentProjects
									.Where(csp => csp.ClassId == classId)
									.Include(csp => csp.User)
									.FirstOrDefaultAsync();

								if (classToAddUserTo != null)
								{
									var userClassAssociation = await _context.ClassStudentProjects
																.Where(csp => csp.ClassId == classId && csp.UserId == existingUser.Id)
																.Include(csp => csp.User)
																.FirstOrDefaultAsync();


									if (userClassAssociation == null)
									{
										// Create a new ClassStudentProject entry for the user
										var newClassStudentProject = new ClassStudentProject
										{
											UserId = existingUser.Id,
											ClassId = classToAddUserTo.ClassId,
											ProjectId = 1,
											Status = classStatus,


											// Set the ProjectId property if applicable
										};

										_context.ClassStudentProjects.Add(newClassStudentProject);
										await _context.SaveChangesAsync(); // Save the new class-user relationship
									}
								}

							}
						} while (reader.NextResult());
						ViewBag.Message = "success";
					}
				}
			}
			else
			{
				ViewBag.Message = "empty";
			}
			ViewBag.ClassId = id;
			return View("/Views/Classroom/UploadExcel.cshtml");
		}

		[HttpGet]
		[Route("/ManageClassroom/ExportExcel")]
		public async Task<FileResult> ExportStudentsToExcel(int id)
		{
			/*var studentInClass = await _context.Classes
							.Where(cls => cls.Id == id)
							.Include(cls => cls.Users) // Eager loading to load associated users
							.Select(cls => cls.Users.Where(user => user.RoleSettingId == 5))
							.FirstOrDefaultAsync();*/

			var studentsInClass = await _context.ClassStudentProjects
									.Where(csp => csp.ClassId == id) // Filter by the class Id
									.Include(csp => csp.User) // Eager loading to load associated users
									.Select(csp => csp.User)
									.Where(user => user.RoleSettingId == 5)
									.ToListAsync();

			var fileName = "ClassID:" + id + ".xlsx";
			return GenerateExcel(fileName, studentsInClass);
		}

		private FileResult GenerateExcel(string fileName, IEnumerable<User> users)
		{
			DataTable dataTable = new DataTable("User");
			dataTable.Columns.AddRange(new DataColumn[]
			{
				new DataColumn("Email"),
				new DataColumn("Name"),
				new DataColumn("Status"),
			});

			foreach (var user in users)
			{
				string genderString = (user.Status.HasValue) ? (user.Status.Value ? "1" : "0") : "Unknown";
				dataTable.Rows.Add(user.Email, user.Name, genderString);
			}

			using (XLWorkbook wb = new XLWorkbook())
			{
				wb.Worksheets.Add(dataTable);
				using (MemoryStream stream = new MemoryStream())
				{
					wb.SaveAs(stream);
					return File(stream.ToArray(),
						"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
						fileName);
				}
			}
		}

		[HttpGet]
		[Route("/ManageClassroom/DownloadTemplate")]
		public IActionResult GetExcelTemplate()
		{
			string path = "wwwroot/Template/Template.xlsx"; // Use a relative path

			if (System.IO.File.Exists(path))
			{
				var fileBytes = System.IO.File.ReadAllBytes(path);
				return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Template.xlsx");
			}

			return RedirectToAction("UploadExcel");
		}


		private bool UserExists(int id)
		{
			return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
		}
	}
}