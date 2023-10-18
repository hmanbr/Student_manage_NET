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
                
                var subjestList = await _context.Subjects.ToListAsync();
                var classlist = await _context.Classes.ToListAsync();

                ClassSubjectViewModel classSubjectViewModel = new ClassSubjectViewModel()
                {
                    Classes = classlist,
                    Subject = subjestList
                };

                // Pass the list of settings to the view.
                return View("/Views/Classroom/ClassesList.cshtml", classSubjectViewModel);
            }
            else
            {
                var classlist = await _context.Classes
                    .Where(classlist => classlist.Id.Equals(search) ||  classlist.Name.Contains(search))
                    .ToListAsync();

                var subjestList = await _context.Subjects.ToListAsync();

                ClassSubjectViewModel classSubjectViewModel = new ClassSubjectViewModel()
                {
                    Classes = classlist,
                    Subject = subjestList
                };
                // Pass the list of settings to the view.
                return View("/Views/Classroom/ClassesList.cshtml", classSubjectViewModel);
            }
            
        }

		[Route("/ManageClassroom/ClassDetail")]
		public async Task<IActionResult> ClassDetail()
        {
			var userList = await _context.Users.Where(userList => userList.RoleSettingId == 5).ToListAsync();
			return View("/Views/Classroom/ClassDetail.cshtml", userList);
		}



	}
}
