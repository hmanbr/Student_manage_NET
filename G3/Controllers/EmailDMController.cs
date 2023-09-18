using Microsoft.AspNetCore.Mvc;

namespace G3.Controllers
{
    public class EmailDMController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Route("/listEmail")]
        public IActionResult ListEmailDM()
        {
            return View();
        }

        [Route("/addEmail")]
        public IActionResult AddNewEmailDM()
        {
            return View();
        }

        [Route("/editEmail")]
        public IActionResult EditEmailDM()
        {
            return View();
        }

        [Route("/detailsEmail")]
        public IActionResult DetailsEmailDM()
        {
            return View();
        }
    }
}
