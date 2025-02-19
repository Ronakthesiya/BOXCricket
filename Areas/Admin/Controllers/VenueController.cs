using BOXCricket.Models;
using BOXCricket.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core.Features;
using Newtonsoft.Json;

namespace BOXCricket.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class VenueController : Controller
    {
        private readonly ApiClientService _apiClientService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly string _apiBaseUrlVenue = "Venue";
        private readonly int? adminId;

        public VenueController(ApiClientService apiClientService, IHttpContextAccessor contextAccessor)
        {
            _apiClientService = apiClientService;
            _contextAccessor = contextAccessor;
            adminId = Convert.ToInt32(_contextAccessor.HttpContext!.Session.GetInt32("id"));
            //adminId = _contextAccessor.HttpContext!.Session.GetInt32("id");
            //adminId = 23;
        }

        public async Task<IActionResult> AllVenues()
        {
            //int? adminId = 23;

            if (adminId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            List<VenueModel> venues = new List<VenueModel>();
            ApiResponseModel response = await _apiClientService.GetAsync($"{_apiBaseUrlVenue}/Admin/{adminId}");

            if(response.StatusCode == 204)
            {
                return View(venues);
            }

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


        public async Task<IActionResult> VenueAddEdit(int venueId)
        {
			VenueModel venue = new VenueModel();
			if (venueId >= 1)
            {
				ApiResponseModel response = await _apiClientService.GetAsync($"{_apiBaseUrlVenue}/{venueId}");
				if (response.StatusCode != 200)
				{
					return NotFound(response.Message);
				}

				venue = JsonConvert.DeserializeObject<VenueModel>(response.Data!.ToString());

			}

            return View(venue);
        }


        [HttpPost]
        public async Task<IActionResult> VenueAddEdit(VenueModel venue, IFormFile VenueImage)
        {

            if (venue.VenueId <= 0)
            {

                venue.AdminId = (int)adminId;

                if (VenueImage.Length > 3 * 1024 * 1024)
                {
                    ModelState.AddModelError("Img", "Image size must be less than 3MB.");
                }
                else
                {
                    // Call method to upload image
                    ApiResponseModel res = await _apiClientService.UploadImage(VenueImage);
                    if (res.StatusCode == 200)
                    {
                        venue.Images = new List<PhotoModel>();
                        venue.Images.Add(new PhotoModel { Photo = res.Data!.ToString() });

                        // remove VenueImage validation error
                        ModelState.Remove("Img");
                    }
                    else
                    {
                        ModelState.AddModelError("Img", "Failed to upload image. Please try again.");
                    }
                }
            

                ApiResponseModel response = await _apiClientService.PostAsync($"{_apiBaseUrlVenue}", venue);
                if (response.StatusCode != 200)
                {
                    return NotFound(response.Message);
                }

                long id = response.Data;

                return RedirectToAction("VenueDetail", new { venueId = (int)id });

            }
            else
            {
                ModelState.Remove("Images");
                ModelState.Remove("VenueImage");
               
                if (ModelState.IsValid)
                {
                    ApiResponseModel response;
                    if (VenueImage == null)
                    {
                        response = await _apiClientService.PutAsync($"{_apiBaseUrlVenue}/{venue.VenueId}", venue);
                    }
                    else
                    {
                        ApiResponseModel res = await _apiClientService.UploadImage(VenueImage);
                        if (res.StatusCode != 200)
                        {
                            return BadRequest(res.Message);
                        }

                        venue.Images[0].Photo = res.Data!.ToString();

                        response = await _apiClientService.PutAsync($"{_apiBaseUrlVenue}/{venue.VenueId}", venue);

                    }
                    
                    if (response.StatusCode != 200)
                    {
                        return NotFound(response.Message);
                    }

                    return RedirectToAction("VenueDetail", new { venueId = venue.VenueId });
                }
            }
        

            return View(venue);
        }


        public async Task<IActionResult> VenueDelete(int venueId)
        {
            if(venueId <= 0)
            {
                return RedirectToAction("AllVenues");
            }

            ApiResponseModel response = await _apiClientService.DeleteAsync($"{_apiBaseUrlVenue}/{venueId}");

            if (response.StatusCode != 200)
            {
                return NotFound(response.Message);
            }

            return RedirectToAction("AllVenues");
        }

    }
}
