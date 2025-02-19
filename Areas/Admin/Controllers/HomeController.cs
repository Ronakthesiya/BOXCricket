using BOXCricket.Models;
using BOXCricket.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BOXCricket.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly ApiClientService _apiClientService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly string _apiBaseUrlVenue = "Venue";
        private readonly string _apiBaseUrlAdmin = "Admin";
        private readonly int adminId;
        public HomeController(ApiClientService apiClientService,IHttpContextAccessor httpContextAccessor)
        {
            _contextAccessor = httpContextAccessor;
            _apiClientService = apiClientService;
            adminId = Convert.ToInt32(_contextAccessor.HttpContext!.Session.GetInt32("id"));
            //adminId = 23;
        }

        public async Task<IActionResult> Index()
        {
            ApiResponseModel response = await _apiClientService.GetAsync(_apiBaseUrlAdmin+ "/Dashboard/"+adminId);

            if(response.StatusCode != 200)
            {
                return NotFound(response.Message);
            }

            DashboardModel dashboardModel = JsonConvert.DeserializeObject<DashboardModel>(response.Data!.ToString());

            return View(dashboardModel);
        }

        public async Task<IActionResult> Profile()
        {

            ApiResponseModel response = await _apiClientService.GetAsync($"{_apiBaseUrlAdmin}/{adminId}");

            if(response.StatusCode != 200)
            {
                return NotFound(response.Message);
            }

            AdminModel admin = JsonConvert.DeserializeObject<AdminModel>(response.Data!.ToString());
            return View(admin);
        }


        public async Task<ActionResult> ProfileEdit(int adminId)
        {
            ApiResponseModel response = await _apiClientService.GetAsync($"{_apiBaseUrlAdmin}/{adminId}");

            if (response.StatusCode != 200)
            {
                return NotFound(response.Message);
            }

            AdminModel admin = JsonConvert.DeserializeObject<AdminModel>(response.Data!.ToString());
            return View(admin);
        }

        [HttpPost]
        public async Task<ActionResult> ProfileEdit(AdminModel admin)
        {
            admin.Role = "Admin";
            ModelState.Remove("PasswordHash");
            ModelState.Remove("Role");
            if (ModelState.IsValid)
            {

                ApiResponseModel response = await _apiClientService.PutAsync($"{_apiBaseUrlAdmin}/{admin.AdminId}", admin);
                    if (response.StatusCode != 200)
                    {
                        return NotFound(response.Message+" not updated");
                    }

                    return RedirectToAction("Profile");

            }

            return NotFound();
        }


    }
}
