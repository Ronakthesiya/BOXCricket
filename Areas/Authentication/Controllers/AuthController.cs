using BOXCricket.Areas.Authentication.Models;
using BOXCricket.Models;
using BOXCricket.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections;

namespace BOXCricket.Areas.Authentication.Controllers
{
    [Area("Authentication")]
    public class AuthController : Controller
    {
        private readonly ApiClientService _apiClientService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _apiBaseUrl = "Account";
        public AuthController(ApiClientService apiClientService,IHttpContextAccessor httpContextAccessor)
        {
            _apiClientService = apiClientService;
            _httpContextAccessor = httpContextAccessor;
        }
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SingInModel signInModel)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "All feilds are required.";
                return View(signInModel);
            }

            Hashtable hashtable = new Hashtable();
            

            ApiResponseModel response = await _apiClientService.PostAsync($"{_apiBaseUrl}/login", signInModel);

            if (response!=null && response.StatusCode == 200)
            {
                Console.WriteLine(response.Data.user.userName);

                return RedirectToAction("SignIn");
            }



            return View(signInModel); 
        }

        public IActionResult Ragister()
        {
            return View();
        }
    }
}
