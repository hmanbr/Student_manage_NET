using Microsoft.AspNetCore.Mvc;

namespace G3.Controllers
{
    public class AdminController : Controller
    {
        [Route("/")]
        public IActionResult AdminHome()
        {
            return View();
        }
    }
}
