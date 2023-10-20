using ExcelDataReader;
using G3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
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
								.Select(cls => cls.Users)
								.FirstOrDefaultAsync();
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
		public async Task<IActionResult> ClassAddStudents()
		{
			var studentList = await _context.Users.Where(userList => userList.RoleSettingId == 5).ToListAsync();
			return View("/Views/Classroom/ClassAddStudents.cshtml", studentList);
		}

		[HttpPost]
		public async Task<IActionResult> AddStudentsToClass(UserMultiSelectViewModel userMultiSelectViewModel)
		{
			List<User> userAddList = new List<User>();

			foreach (var selectedUser in userMultiSelectViewModel.UsersSelectList)
			{
				var selectedUserList = _context.Users.Where(user => user.Id.Equals(selectedUser.User.Id)).FirstOrDefault();
				if (selectedUserList != null)
				{
					userAddList.Add(selectedUserList);
				}

			}
			IEnumerable<User> UserAddListEnu = userAddList;
			return View("/Views/Classroom/ClassStudents.cshtml", UserAddListEnu);
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

			if(file != null && file.Length > 0)
			{
				var uploadFolder = $"{Directory.GetCurrentDirectory()}\\wwwroot\\Uploads";
				if(!Directory.Exists(uploadFolder))
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
								if(!isHeaderSkipped)
								{
									isHeaderSkipped = true;
									continue;
								}
								User user = new User();
								user.Email = reader.GetValue(0).ToString();

								if(user.Email.Contains("gmail.com")) {
									user.DomainSettingId = 7;
								}else if(user.Email.Contains("fpt.edu.vn"))
								{
									user.DomainSettingId = 6;
								}

								user.Name = reader.GetValue(1).ToString();
								user.RoleSettingId = 5;
								user.Status = true;
								if(reader.GetValue(2).ToString().Equals("Male"))
								{
									user.Gender = true;
								}else
								{
									user.Gender = false;
								}

								bool existed = false;
								var userList = await _context.Users.ToListAsync();
								foreach(var existedUser in userList)
								{
									if(existedUser.Email.Equals(user.Email))
									{
										existed = true;
									}
								}
								
								if(!existed)
								{
									_context.Add(user);
								}


								int classId = id;
								var classToAddUserTo = await _context.Classes
															.Where(c => c.Id == classId)
															.Include(c => c.Users)
															.FirstOrDefaultAsync();
								string emailToCheck = user.Email;
								bool userExistsInClass = classToAddUserTo.Users.Any(user => user.Email == emailToCheck);

								if (classToAddUserTo != null && !userExistsInClass)
								{
									// Add the user to the class
									classToAddUserTo.Users.Add(user);
								}
								await _context.SaveChangesAsync();
							}
						} while (reader.NextResult());
					}
				}
			}
			ViewBag.ClassId = id;
			return View("/Views/Classroom/UploadExcel.cshtml");
		}
	}

}
