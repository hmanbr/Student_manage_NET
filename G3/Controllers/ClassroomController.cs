using ClosedXML.Excel;
using ExcelDataReader;
using G3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Data;
using System.Text;

namespace G3.Controllers
{
	public class ClassroomController : Controller
	{
		private readonly SWPContext _context;

		public ClassroomController(SWPContext context)
		{
			_context = context;
		}

		[Route("/ManageClassroom/ClassesList")]
		public async Task<IActionResult> ClassList(string search)
		{
			if (string.IsNullOrEmpty(search))
			{
				var classlist = await _context.Classes.Include(subject => subject.Subject).ToListAsync();
				// Pass the list of settings to the view.
				return View("/Views/Classroom/ClassesList.cshtml", classlist);
			}
			else
			{
				var classlist = await _context.Classes.Include(subject => subject.Subject)
								.Where(classlist => classlist.Id.Equals(search) || classlist.Name.Contains(search) || classlist.Subject.SubjectCode.Contains(search))
								.ToListAsync();

				// Pass the list of settings to the view.
				return View("/Views/Classroom/ClassesList.cshtml", classlist);
			}

		}

		[Route("/ManageClassroom/ClassDetail")]
		public async Task<IActionResult> ClassDetail(int id)
		{
			var usersInClass = await _context.Classes
							.Where(cls => cls.Id == id)
							.Include(cls => cls.Users) // Eager loading to load associated users
							.SelectMany(cls => cls.Users)
							.Take(5) // Limit to the first 5 users
							.ToListAsync();

			ViewBag.ClassId = id;
			return View("/Views/Classroom/ClassDetail.cshtml", usersInClass);
		}

		[Route("/ManageClassroom/ClassStudents")]
		public async Task<IActionResult> ClassStudents(int id)
		{
			var usersInClass = await _context.Classes
							.Where(cls => cls.Id == id)
							.Include(cls => cls.Users) // Eager loading to load associated users
							.Select(cls => cls.Users.Where(user => user.RoleSettingId == 5))
							.FirstOrDefaultAsync();
			ViewBag.ClassId = id;
			return View("/Views/Classroom/ClassStudents.cshtml", usersInClass);
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

			var classToAddUserTo = await _context.Classes
				.Where(c => c.Id == classId)
				.Include(c => c.Users)
				.FirstOrDefaultAsync();


			if (classToAddUserTo != null && !classToAddUserTo.Users.Any(u => u.Email == existingUser.Email))
			{
				// Add the user to the class
				classToAddUserTo.Users.Add(existingUser);
				await _context.SaveChangesAsync(); // Save the class-user relationship
			}
			ViewBag.ClassId = classId;
			return RedirectToAction("ClassStudents", "Classroom", new { id = classId });
		}

		[Route("/ManageClassroom/UploadExcel")]
		public async Task<IActionResult> UploadExcel(int id)
		{
			ViewBag.ClassId = id;
			return View("/Views/Classroom/UploadExcel.cshtml");
		}

		[Route("/ManageClassroom/UploadExcel")]
		[HttpPost]
		public async Task<IActionResult> UploadExcel(IFormFile file, int id)
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
								user.Email = reader.GetValue(0).ToString();

								if (user.Email.Contains("gmail.com"))
								{
									user.DomainSettingId = 7;
								}
								else if (user.Email.Contains("fpt.edu.vn"))
								{
									user.DomainSettingId = 6;
								}

								user.Name = reader.GetValue(1).ToString();
								user.RoleSettingId = 5;
								user.Status = true;
								if (reader.GetValue(2).ToString().Equals("Male"))
								{
									user.Gender = true;
								}
								else
								{
									user.Gender = false;
								}


								string emailToCheck = user.Email;

								// Check if the user already exists in the database
								User existingUser = _context.Users.FirstOrDefault(u => u.Email == emailToCheck);

								if (existingUser == null)
								{
									_context.Add(user);
									await _context.SaveChangesAsync(); // Save the new user
									existingUser = user; // Update existingUser with the newly added user
								}


								int classId = id;
								var classToAddUserTo = await _context.Classes
									.Where(c => c.Id == classId)
									.Include(c => c.Users)
									.FirstOrDefaultAsync();

								if (classToAddUserTo != null && !classToAddUserTo.Users.Any(u => u.Email == emailToCheck))
								{
									// Add the user to the class
									classToAddUserTo.Users.Add(existingUser);
									await _context.SaveChangesAsync(); // Save the class-user relationship
								}
							}
						} while (reader.NextResult());
					}
				}
			}
			ViewBag.ClassId = id;
			return View("/Views/Classroom/UploadExcel.cshtml");
		}

		[HttpGet]
		[Route("/ManageClassroom/ExportExcel")]
		public async Task<FileResult> ExportStudentsToExcel(int id)
		{
			var studentInClass = await _context.Classes
							.Where(cls => cls.Id == id)
							.Include(cls => cls.Users) // Eager loading to load associated users
							.Select(cls => cls.Users.Where(user => user.RoleSettingId == 5))
							.FirstOrDefaultAsync();
			var fileName = "ClassID:" + id + ".xlsx";
			return GenerateExcel(fileName, studentInClass);
		}

		private FileResult GenerateExcel(string fileName, IEnumerable<User> users)
		{
			DataTable dataTable = new DataTable("User");
			dataTable.Columns.AddRange(new DataColumn[]
			{
				new DataColumn("Email"),
				new DataColumn("Name"),
				new DataColumn("Gender"),
			});

			foreach (var user in users)
			{
				string genderString = (user.Gender.HasValue) ? (user.Gender.Value ? "Male" : "Female") : "Unknown";
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
	}

}
