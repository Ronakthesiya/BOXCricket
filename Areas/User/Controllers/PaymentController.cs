using BOXCricket.Models;
using BOXCricket.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Configuration;

namespace BOXCricket.Areas.User.Controllers
{
    [Area("User")]
    public class PaymentController : Controller
    {
        private readonly ApiClientService _apiClientService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly string _apiBaseUrlPayment = "Payment";
        private readonly IConfiguration _configuration;
        private readonly int? userId;

        public PaymentController(ApiClientService apiClientService, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _apiClientService = apiClientService;
            _contextAccessor = httpContextAccessor;
            _configuration = configuration;
            userId = _contextAccessor.HttpContext.Session.GetInt32("id");
        }
        public async Task<IActionResult> Payments(int? venueId)
        {

            if (venueId == null) { venueId = -1; }

            ApiResponseModel responce = await _apiClientService.GetAsync($"{_apiBaseUrlPayment}/Ids/{venueId}/{userId}");
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
