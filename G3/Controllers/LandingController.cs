using Microsoft.AspNetCore.Mvc;

namespace G3.Controllers
{
    public class LandingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Route("/landingPage")]
        public IActionResult ViewLanding() { 
        
            return View();
        }
    }
}
