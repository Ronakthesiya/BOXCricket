using BOXCricket.Models;
using BOXCricket.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BOXCricket.Areas.User.Controllers
{
    [Area("User")]
    public class HomeController : Controller
    {
        private readonly int? _userId;
        private readonly String? _userName;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ApiClientService _apiClientService;
        private readonly string _apiBaseUrlVenue = "Venue";

        public HomeController(IHttpContextAccessor httpContextAccessor,ApiClientService apiClientService)
        {
            _contextAccessor = httpContextAccessor;
            _userId = _contextAccessor.HttpContext!.Session.GetInt32("id");
            _userName = _contextAccessor.HttpContext!.Session.GetString("userName");
            _apiClientService = apiClientService;

        }

        public async Task<IActionResult> Index()
        {
            List<VenueModel> venues = new List<VenueModel>();
            ApiResponseModel response = await _apiClientService.GetAsync($"{_apiBaseUrlVenue}");
            if (response.StatusCode != 200)
            {
                return NotFound(response.Message);
            }

            venues = JsonConvert.DeserializeObject<List<VenueModel>>(response.Data!.ToString());
            ViewBag.Venues = venues;
            TempData["userName"] = _contextAccessor.HttpContext!.Session.GetString("userName");
            return View();
        }

        public IActionResult Profile()
        {
            return View();
        }
    }
}
