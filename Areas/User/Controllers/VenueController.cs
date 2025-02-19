using BOXCricket.Models;
using BOXCricket.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BOXCricket.Areas.User.Controllers
{
    [Area("User")]
    public class VenueController : Controller
    {
        private readonly ApiClientService _apiClientService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly string _apiBaseUrlVenue = "Venue";
        private readonly int? userId;

        public VenueController(ApiClientService apiClientService, IHttpContextAccessor contextAccessor)
        {
            _apiClientService = apiClientService;
            _contextAccessor = contextAccessor;
            userId = _contextAccessor.HttpContext!.Session.GetInt32("id");
            //userId = 23;
        }

        public async Task<IActionResult> AllVenues()
        {
            if (userId == null)
            {
                return RedirectToAction("Index", "Home" ,"Authenticatoin");
            }

            List<VenueModel> venues = new List<VenueModel>();
            ApiResponseModel response = await _apiClientService.GetAsync($"{_apiBaseUrlVenue}");
            if (response.StatusCode != 200)
            {
                return NotFound(response.Message);
            }

            venues = JsonConvert.DeserializeObject<List<VenueModel>>(response.Data!.ToString());

            return View(venues);
        }

        public async Task<IActionResult> VenueDetail(int venueId)
        {

            if (venueId == null)
            {
                return RedirectToAction("AllVenues");
            }

            VenueModel venue = new VenueModel();
            ApiResponseModel response = await _apiClientService.GetAsync($"{_apiBaseUrlVenue}/{venueId}");
            if (response.StatusCode != 200)
            {
                return NotFound(response.Message);
            }

            venue = JsonConvert.DeserializeObject<VenueModel>(response.Data!.ToString());
            

            return View(venue);
        }

        public IActionResult Demo()
        {
            return View();
        }


    }
}
