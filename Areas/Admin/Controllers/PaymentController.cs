using BOXCricket.Models;
using BOXCricket.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BOXCricket.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PaymentController : Controller
    {
        private readonly ApiClientService _apiClientService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly string _apiBaseUrlPayment = "Payment";
        private readonly IConfiguration _configuration;
        private readonly int? adminId;

        public PaymentController(ApiClientService apiClientService, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _apiClientService = apiClientService;
            _contextAccessor = httpContextAccessor;
            _configuration = configuration;
            adminId = Convert.ToInt32(_contextAccessor.HttpContext!.Session.GetInt32("id"));

            //userId = _contextAccessor.HttpContext.Session.GetInt32("id");
        }
        public async Task<IActionResult> AllPayments(int? venueId,int? userId,string? userName)
        {
            if (venueId == null) { venueId = -1; }
            if(userId == null) { userId = -1; }

            if (userId != null)
            {
                TempData["UserNameForPayment"]=userName;
            }


            ApiResponseModel responce = await _apiClientService.GetAsync($"{_apiBaseUrlPayment}/Ids/{venueId}/{userId}/{adminId}");
            List<PaymentModel> payments = new List<PaymentModel>();

            

            if (responce.StatusCode != 200)
            {
                return NotFound(responce.Message);
            }

            payments = JsonConvert.DeserializeObject<List<PaymentModel>>(responce.Data!.ToString());

            return View(payments);
        }
    }
}
