using BOXCricket.Areas.Authentication.Models;
using BOXCricket.Models;
using BOXCricket.Repository;
using BOXCricket.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
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


            ApiResponseModel response = await _apiClientService.PostAsync($"{_apiBaseUrl}/login", signInModel);
            
            if (response!=null && response.StatusCode == 200)
            {
                string userName = response.Data!.user.userName;
                string email = response.Data!.user.email;
                string userId = response.Data!.user.id;
                string userRole = response.Data!.role[0];
                int id = response.Data!.id;


                _httpContextAccessor.HttpContext!.Session.SetString("userName", email);
                _httpContextAccessor.HttpContext!.Session.SetString("email", userName);
                _httpContextAccessor.HttpContext!.Session.SetString("userId", userId);
                _httpContextAccessor.HttpContext!.Session.SetString("userRole", userRole);
                _httpContextAccessor.HttpContext!.Session.SetInt32("id", id);

                string token = response.Data!.token ?? string.Empty;

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.Now.AddHours(1)
                };

                _httpContextAccessor.HttpContext.Response.Cookies.Append("token",token, cookieOptions);

                TempData["success"] = response.Message;

                if (userRole == "Admin") return RedirectToAction("Index" , "Home" , new { area = "Admin" });
                else if(userRole == "User") return RedirectToAction("Index", "Home", new { area = "User" });
            }

            return View(signInModel); 
        }

        public IActionResult Ragister()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Ragister(SignUpModel signUpModel)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "All feilds are required.";
                return View(signUpModel);
            }
            signUpModel.Role = "Admin";
            ApiResponseModel response = await _apiClientService.PostAsync($"{_apiBaseUrl}/register", signUpModel);

            if (response != null && response.StatusCode == 200) {
                ApiResponseModel res = await _apiClientService.PostAsync($"{_apiBaseUrl}/assign-role", signUpModel);

                if(res != null && res.StatusCode == 200)
                {
                    SingInModel singInModel = new SingInModel();
                    singInModel.Email = signUpModel.Email;
                    singInModel.Password = signUpModel.Password;
                    singInModel.Name = signUpModel.Name;

                    return View("SignIn", singInModel);
                }
            }

            return View(signUpModel);
        }

        public IActionResult Logout()
        {
            _httpContextAccessor.HttpContext!.Session.Clear();
            _httpContextAccessor.HttpContext.Response.Cookies.Delete("token");
            return View("SignIn");
        }
    }
}
