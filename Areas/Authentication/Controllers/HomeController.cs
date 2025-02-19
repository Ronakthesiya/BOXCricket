using Microsoft.AspNetCore.Mvc;

namespace BOXCricket.Areas.Authentication.Controllers
{
    [Area("Authentication")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {

            return View();
        }
    }
}
