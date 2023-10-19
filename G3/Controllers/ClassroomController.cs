using G3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
								.Where(classlist => classlist.Id.Equals(search) || classlist.Name.Contains(search) || classlist.Subject.SubjectCode.Equals(search))
								.ToListAsync();

				// Pass the list of settings to the view.
				return View("/Views/Classroom/ClassesList.cshtml", classlist);
			}

		}

		[Route("/ManageClassroom/ClassDetail")]
		public async Task<IActionResult> ClassDetail()
		{
			var studentList = await _context.Users.Where(userList => userList.RoleSettingId == 5).ToListAsync();
			return View("/Views/Classroom/ClassDetail.cshtml", studentList);
		}

		[Route("/ManageClassroom/ClassStudents")]
		public async Task<IActionResult> ClassStudents()
		{
			var studentList = await _context.Users.Where(userList => userList.RoleSettingId == 5).ToListAsync();
			return View("/Views/Classroom/ClassStudents.cshtml", studentList);
		}

		[Route("/ManageClassroom/ClassAddStudents")]
		public async Task<IActionResult> ClassAddStudents()
		{
			var studentList = await _context.Users.Where(userList => userList.RoleSettingId == 5).ToListAsync();
			UserMultiSelectViewModel MultiSelecviewModel = new UserMultiSelectViewModel()
			{
				UsersSelectList = new List<UserSelectViewModel>(),
				SelectAll = false

			};
			foreach (var student in studentList)
			{
				UserSelectViewModel userSelectViewModel = new UserSelectViewModel()
				{
					User = student,
					Selected = false
				};
				MultiSelecviewModel.UsersSelectList.Add(userSelectViewModel);
			}


			return View("/Views/Classroom/ClassAddStudents.cshtml", MultiSelecviewModel);
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
	}

}
