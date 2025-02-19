using Microsoft.AspNetCore.Mvc;

namespace BOXCricket.Areas.User.Controllers
{
    [Area("User")]
    public class HomeController : Controller
    {
        private readonly int? _userId;
        private readonly String? _userName;
        private readonly IHttpContextAccessor _contextAccessor;

        public HomeController(IHttpContextAccessor httpContextAccessor)
        {
            _contextAccessor = httpContextAccessor;
            _userId = _contextAccessor.HttpContext!.Session.GetInt32("id");
            _userName = _contextAccessor.HttpContext!.Session.GetString("userName");
        }

        public IActionResult Index()
        {
            TempData["userName"] = _contextAccessor.HttpContext!.Session.GetString("userName");
            return View();
        }

        public IActionResult Profile()
        {
            return View();
        }
    }
}
