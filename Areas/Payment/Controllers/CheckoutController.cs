using BOXCricket.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json.Nodes;

namespace BOXCricket.Areas.Payment.Controllers
{
    [Area("Payment")]
    public class CheckoutController : Controller
    {
        private string PaypalClientId { get; set; } = "";
        private string PaypalSecret { get; set; } = "";
        private string PaypalUrl { get; set; } = "";

        public CheckoutController(IConfiguration configuration)
        {
            PaypalClientId = configuration["PayPalOptions:ClientId"];
            PaypalSecret = configuration["PayPalOptions:Secret"];
            PaypalUrl = configuration["PayPalOptions:Url"];

        }

        public IActionResult Index()
        {
            ViewBag.PaypalClientId = PaypalClientId;
            return View();
        }

        //public async Task<string> Token()
        //{
        //    return await GetPaypalAccessToken();
        //}


        private async Task<string> GetPaypalAccessToken() {
            string accessToken = "";
            string url = PaypalUrl + "/v1/oauth2/token";

            using(var client = new HttpClient())
            {
                string credentials64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(PaypalClientId + ":" + PaypalSecret));

                client.DefaultRequestHeaders.Add("Authorization", "Basic " + credentials64);

                var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
                requestMessage.Content = new StringContent("grant_type=client_credentials", null, "application/x-www-form-urlencoded");

                var httpResponse = await client.SendAsync(requestMessage);

                if (httpResponse.IsSuccessStatusCode) { 
                    var strResponse = await httpResponse.Content.ReadAsStringAsync();

                    var jsonResponse = JsonNode.Parse(strResponse);
                    if(jsonResponse != null)
                    {
                        accessToken = jsonResponse["access_token"]?.ToString() ?? "";
                    }
                }
            }

            return accessToken;
        }


        [HttpPost]
        public async Task<JsonResult> CreateOrder([FromBody] JsonObject data)
        {
            var totalAmount = data?["amount"]?.ToString();
            if (totalAmount == null)
            {
                return new JsonResult(new { Id = "" });
            }


            //create request body
            JsonObject createOrderRequest = new JsonObject();
            createOrderRequest.Add("intent", "CAPTURE");

            JsonObject amount = new JsonObject();
            amount.Add("currency_code", "USD");
            amount.Add("value", totalAmount);

            JsonObject purchaseUnit1 = new JsonObject();
            purchaseUnit1.Add("amount", amount);

            JsonArray purchaseUnits = new JsonArray();
            purchaseUnits.Add(purchaseUnit1);

            createOrderRequest.Add("purchase_units", purchaseUnits);

            //get access token
            string accessToken = await GetPaypalAccessToken();

            //send request
            string url = PaypalUrl + "/v2/checkout/orders";


            using (var client =  new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
                requestMessage.Content = new StringContent(createOrderRequest.ToString(), null, "application/json");

                var httpResponse = await client.SendAsync(requestMessage);

                if (httpResponse.IsSuccessStatusCode)
                {
                    var strResponse = await httpResponse.Content.ReadAsStringAsync();
                    var jsonResponce = JsonNode.Parse(strResponse);

                    if(jsonResponce != null)
                    {
                        string paypalOrderId = jsonResponce["id"]?.ToString() ?? "";

                        return new JsonResult(new { Id = paypalOrderId });
                    }

                }
            }

                return new JsonResult(new { Id = "" });
        }

        [HttpPost]
        public async Task<IActionResult> CompleteOrder([FromBody] JsonObject data)
        {
            var orderId = data?["orderID"]?.ToString();
            if (orderId == null)
            {
                return new JsonResult("error");
            }

            string accessToken =  await GetPaypalAccessToken();

            string url = PaypalUrl + "/v2/checkout/orders/" + orderId + "/capture";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
                requestMessage.Content = new StringContent("", null, "application/json");

                var httpResponse = await client.SendAsync(requestMessage);

                if (httpResponse.IsSuccessStatusCode)
                {
                    var strResponse = await httpResponse.Content.ReadAsStringAsync();
                    var jsonResponce = JsonNode.Parse(strResponse);

                    if (jsonResponce != null)
                    {
                        string paypalOrderId = jsonResponce["status"]?.ToString() ?? "";

                        if(paypalOrderId == "COMPLETED")
                        {
                            //save the order in database
                            //BookingModel bookingModel = JsonConvert.DeserializeObject<BookingModel>(data?["bookingModel"]?.ToString());

                            //RedirectToAction("ConfirmBooking", "Booking", new { area = "User", bookingModel = bookingModel });
                            //return Json(new { status = "success", redirectUrl = Url.Action("ConfirmBooking", "Booking", new { area = "User" }) });

                            return new JsonResult("success");
                        }
                    }
                }
            }

            return new JsonResult("error");
        }

    }
}
