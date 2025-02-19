using BOXCricket.Models;
using BOXCricket.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BOXCricket.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly string _apiBaseUrlUser = "User";
        private readonly ApiClientService _apiClientService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly int? adminId;
        public UserController(ApiClientService apiClientService,IHttpContextAccessor httpContextAccessor)
        {
            _apiClientService = apiClientService;
            _contextAccessor = httpContextAccessor;
            adminId = Convert.ToInt32(_contextAccessor.HttpContext!.Session.GetInt32("id"));
            //adminId = _contextAccessor.HttpContext!.Session.GetInt32("id");
            //adminId = 23;
        }
        public async Task<IActionResult> AllUsers()
        {
            ApiResponseModel res = await _apiClientService.GetAsync($"{_apiBaseUrlUser}/Admin/{adminId}");
            List<UserModel> users = new List<UserModel>();

            if(res.StatusCode == 204)
            {
                return View(users);
            }

            if (res.StatusCode != 200)
            {
                return NotFound(res.Message);
            }

            users = JsonConvert.DeserializeObject<List<UserModel>>(res.Data!.ToString());

            return View(users);
        }
    }
}
