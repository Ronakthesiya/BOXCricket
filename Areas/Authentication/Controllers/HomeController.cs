using BOXCricket.Models;
using BOXCricket.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BOXCricket.Areas.Authentication.Controllers
{
    [Area("Authentication")]
    public class HomeController : Controller
    {
        private readonly ApiClientService _apiClientService;
        private readonly string _apiBaseUrlVenue = "Venue";

        public HomeController(ApiClientService apiClientService)
        {
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

            return View();
        }
    }
}
