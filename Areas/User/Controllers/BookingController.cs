using BOXCricket.Models;
using BOXCricket.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Text.Json.Nodes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BOXCricket.Areas.User.Controllers
{
    [Area("User")]
    public class BookingController : Controller
    {
        private readonly ApiClientService _apiClientService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly string _apiBaseUrlBooking = "Booking";
        private readonly string _apiBaseUrlVenue = "Venue";
        private readonly string _apiBaseUrlUser = "User";
        private readonly string _apiBaseUrlPayment = "Payment";
        private readonly IConfiguration _configuration;
        private readonly int? userId;

        public BookingController(ApiClientService apiClientService, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _apiClientService = apiClientService;
            _contextAccessor = httpContextAccessor;
            _configuration = configuration;
            userId = _contextAccessor.HttpContext.Session.GetInt32("id");
            //userId = 1;
        }

        public async Task<IActionResult> BookingByVenue(int venueId)
        {
            await BookedSloatByVenueAndDate(venueId, DateTime.Now);

            ViewData["VenueIdForTimeBar"] = venueId;
            ApiResponseModel responce = await _apiClientService.GetAsync($"{_apiBaseUrlBooking}/Admin/{-1}/{venueId}/{userId}");
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

            TempData["venueId"] = venueId;
            return View(bookings);
        }


        [HttpPost]
        public async Task<List<string>> BookedSloatByVenueAndDate(int venueId, DateTime date)
        {
            ApiResponseModel response = await _apiClientService.PostAsync($"{_apiBaseUrlBooking}/sloats/{venueId}", date);
            List<List<TimeSpan>> sloats = new List<List<TimeSpan>>();

            if (response.StatusCode != 200)
            {
                return null;
            }

            sloats = JsonConvert.DeserializeObject<List<List<TimeSpan>>>(response.Data!.ToString());

            List<string> hours = new List<string>();

            foreach (var sloat in sloats)
            {
                for (int i = sloat[0].Hours; i < sloat[1].Hours; i++)
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


        public async Task<IActionResult> BookingsByUser()
        {
            List<BookingModel> bookings = new List<BookingModel>();

            ApiResponseModel res = await _apiClientService.GetAsync($"{_apiBaseUrlBooking}/Admin/{-1}/{-1}/{userId}");

            if(res.StatusCode != 200)
            {
                return NoContent();
            }

            bookings = JsonConvert.DeserializeObject<List<BookingModel>>(res.Data!.ToString());

            return View(bookings);

        }

        public async Task<IActionResult> AddBooking([FromForm] TimeSpan StartTime, [FromForm] TimeSpan EndTime, [FromForm] DateTime dateInput) {

            if (userId == null)
            {
                return NoContent();
            }
            BookingModel bookingModel = new BookingModel();

            bookingModel.VenueId = Convert.ToInt32(TempData.Peek("venueId"));
            bookingModel.StartTime = StartTime;
            bookingModel.EndTime = EndTime;
            bookingModel.UserId = userId ?? 0;
            bookingModel.BookingDate = dateInput;
            bookingModel.Status = "Pending";
            bookingModel.Venue = new VenueModel();
            bookingModel.User = new UserModel();

            //check sloat available
            if (StartTime >= EndTime)
            {
                TempData["Error"] = "Enter Vailed time and date";
                return RedirectToAction("BookingByVenue", new { venueId = bookingModel.VenueId });
            }
            if (!await CheckSlotIsBookedOrNot(StartTime, EndTime, bookingModel.VenueId, dateInput))
            {
                TempData["Error"] = "Slote is Already Booked";
                return RedirectToAction("BookingByVenue", new { venueId = bookingModel.VenueId });
            }

           


            ApiResponseModel response = await _apiClientService.GetAsync($"{_apiBaseUrlVenue}/{bookingModel.VenueId}");
            if (response.StatusCode != 200)
            {
                return NotFound(response.Message);
            }

            bookingModel.Venue = JsonConvert.DeserializeObject<VenueModel>(response.Data!.ToString());

            //paypal ClientId
            ViewBag.PaypalClientId = _configuration["PayPalOptions:ClientId"];

            return View(bookingModel);
        }

        //[HttpPost]
        //public async Task<IActionResult> ConfirmBooking(BookingModel bookingModel)
        //{
        //    if (userId == null)
        //    {
        //        return NoContent();
        //    }

        //    bookingModel.UserId = userId ?? 0;
        //    bookingModel.Status = "Confirmed";

        //    ApiResponseModel res = await _apiClientService.PostAsync($"{_apiBaseUrlBooking}", bookingModel);

        //    if (res.StatusCode != 200)
        //    {
        //        TempData["Error"] = res.Data.Message;

        //        return NoContent();
        //    }


        //    return RedirectToAction("BookingByVenue", new { venueId = bookingModel.VenueId });
        //    //return RedirectToAction("BookingsByUser");
        //}

        [HttpPost]
        public async Task<IActionResult> ConfirmBooking([FromBody] JsonObject data)
        {
           BookingModel bookingModel = JsonConvert.DeserializeObject<BookingModel>(data?["bookingModel"]?.ToString());
            if (userId == null)
            {
                return NoContent();
            }

            bookingModel.UserId = userId ?? 0;
            bookingModel.Status = "Confirmed";
            bookingModel.TotalAmount = Convert.ToDecimal(((bookingModel.EndTime.TotalMinutes - bookingModel.StartTime.TotalMinutes)/60.00)*bookingModel.Venue.PricePerHour);

            ApiResponseModel res = await _apiClientService.PostAsync($"{_apiBaseUrlBooking}", bookingModel);

            if (res.StatusCode != 200)
            {
                TempData["Error"] = res.Data.Message;

                return NoContent();
            }

            PaymentModel payment = new PaymentModel();
            payment.BookingId = Convert.ToInt32(res.Data.bookingId);
            payment.UserId = userId ?? 0;
            payment.VenueId = bookingModel.VenueId;
            payment.AmountPaid = bookingModel.TotalAmount;
            payment.TransactionId = data?["orderID"]?.ToString();

            ApiResponseModel res2 = await _apiClientService.PostAsync($"{_apiBaseUrlPayment}", payment);

            if (res2.StatusCode != 200)
            {
                TempData["Error"] = res.Data?.Message;

                return NoContent();
            }



            return RedirectToAction("BookingByVenue", new { venueId = bookingModel.VenueId });
            //return RedirectToAction("BookingsByUser");
        }


        public async Task<bool> CheckSlotIsBookedOrNot(TimeSpan starttime , TimeSpan endtime , int venueId , DateTime date)
        {

            ApiResponseModel response = await _apiClientService.PostAsync($"{_apiBaseUrlBooking}/sloats/{venueId}", date);
            List<List<TimeSpan>> sloats = new List<List<TimeSpan>>();

            if (response.StatusCode != 200)
            {
                return false;
            }

            sloats = JsonConvert.DeserializeObject<List<List<TimeSpan>>>(response.Data!.ToString());


            if (starttime < endtime)
            {
                foreach (var sloat in sloats)
                {
                    if ((starttime < sloat[0] && sloat[0] < endtime) || (sloat[0]<endtime && endtime < sloat[1]) || (sloat[0] < starttime && starttime < sloat[1]) || (starttime < sloat[1] && sloat[1] < endtime) || (starttime == sloat[0] && endtime == sloat[1]))
                    {
                        return false;
                    }
                }
            }
            else return false;
            

            return true;


            //if(starttime < endtime)
            //{
            //    foreach (var sloat in sloats)
            //    {
            //        if (sloat[0] < sloat[1])
            //        {
            //            if ((starttime < sloat[0] && sloat[0] < endtime) || (starttime < sloat[1] && sloat[1] < endtime) || (starttime == sloat[0] && endtime == sloat[1]))
            //            {
            //                return false;
            //            }
            //        }
            //        else
            //        {
            //            //code..
            //        }
            //    }
            //}
            //else
            //{
            //    foreach(var sloat in sloats)
            //    {
            //        if (sloat[0] < sloat[1])
            //        {
            //            if ((1<=sloat[0].Hours && sloat[0] < starttime) || 
            //                (endtime <= sloat[0] && sloat[0].Hours < 24) ||
            //                (1 < sloat[1].Hours && sloat[1] <= starttime) || 
            //                (endtime < sloat[1] && sloat[1].Hours <= 24))
            //            {
            //                return false;
            //            }
            //        }
            //        else
            //        {
            //            //code ..
            //        }
            //    }
            //}

        }

    }
}
