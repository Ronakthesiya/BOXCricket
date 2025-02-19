using BOXCricket.Models;
using BOXCricket.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Newtonsoft.Json;
using System.ComponentModel;

namespace BOXCricket.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BookingController : Controller
    {
        private readonly ApiClientService _apiClientService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly string _apiBaseUrlBooking = "Booking";
        private readonly string _apiBaseUrlVenue = "Venue";
        private readonly string _apiBaseUrlUser = "User";
        private readonly int? adminId;

        public BookingController(ApiClientService apiClientService,IHttpContextAccessor httpContextAccessor)
        {
            _apiClientService = apiClientService;
            _contextAccessor = httpContextAccessor;
            adminId = Convert.ToInt32(_contextAccessor.HttpContext!.Session.GetInt32("id"));
            //adminId = _contextAccessor.HttpContext!.Session.GetInt32("id");
            //adminId = 23;
        }
        public async Task<IActionResult> AllBooking()
        {
            ApiResponseModel responce = await _apiClientService.GetAsync($"{_apiBaseUrlBooking}/Admin/{adminId}");
            List<BookingModel> bookings = new List<BookingModel>();

            if (responce.StatusCode != 200) { 
                return NotFound(responce.Message);
            }

            bookings = JsonConvert.DeserializeObject<List<BookingModel>>(responce.Data!.ToString());

            return View(bookings);
        }


        public async Task<IActionResult> BookingByVenue(int venueId)
        {
            await BookedSloatByVenueAndDate(venueId,DateTime.Now);

            ViewData["VenueIdForTimeBar"] = venueId;
            ApiResponseModel responce = await _apiClientService.GetAsync($"{_apiBaseUrlBooking}/Admin/{adminId}/{venueId}");
            List<BookingModel> bookings = new List<BookingModel>();

            if (responce.StatusCode != 200)
            {
                return NotFound(responce.Message);
            }

            bookings = JsonConvert.DeserializeObject<List<BookingModel>>(responce.Data!.ToString());
            if (bookings.Count > 0)
            {
                ViewData["VenueName"] = bookings[0].Venue.Name;

            }
            else
            {
                ApiResponseModel response1 = await _apiClientService.GetAsync($"{_apiBaseUrlVenue}/{venueId}");
                VenueModel venue = JsonConvert.DeserializeObject<VenueModel>(response1.Data!.ToString());
                ViewData["VenueName"] = venue.Name;
            }

            return View(bookings);
        }

        [HttpPost]
        public async Task<List<String>> BookedSloatByVenueAndDate(int venueId, DateTime date)
        {
            ApiResponseModel response = await _apiClientService.PostAsync($"{_apiBaseUrlBooking}/sloats/{venueId}", date);
            List<List<TimeSpan>> sloats = new List<List<TimeSpan>>();

            if (response.StatusCode != 200)
            {
                return null;
            }

            sloats = JsonConvert.DeserializeObject<List<List<TimeSpan>>>(response.Data!.ToString());
            
            List<String> hours = new List<String>();
                        
            foreach (var sloat in sloats)
            {
                for(int i = sloat[0].Hours; i < sloat[1].Hours; i++)
                {
                    hours.Add(i.ToString() + ":00");
                }
            }

            TempData["BookedSloats"] = hours;

            return hours;


        }


        [HttpPost]
        public async Task<IActionResult> GetBookedSloats(int venueId, DateTime date)
        {
            await BookedSloatByVenueAndDate(venueId, date);
            var bookedSloats = TempData["BookedSloats"] as List<string>;
            return Json(bookedSloats ?? new List<string>());
        }


        public async Task<IActionResult> BookingByUser(int userId)
        {
            ApiResponseModel response = await _apiClientService.GetAsync($"{_apiBaseUrlBooking}/Admin/{adminId}/-1/{userId}");
            List<BookingModel> bookings = new List<BookingModel>();

            if (response.StatusCode != 200)
            {
                return NotFound(response.Message);
            }

            bookings = JsonConvert.DeserializeObject<List<BookingModel>>(response.Data!.ToString());

            if (bookings.Count > 0)
            {
                TempData["UserName"] = bookings[0].User.Name;
            }
            else
            {
                return NotFound();
            }
            return View(bookings);
        }


    }
}
