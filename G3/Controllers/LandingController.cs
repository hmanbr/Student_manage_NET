using Microsoft.AspNetCore.Mvc;

namespace G3.Controllers
{
    public class LandingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Route("/home")]
        public IActionResult ViewLanding() { 
        
            return View();
        }
    }
}
