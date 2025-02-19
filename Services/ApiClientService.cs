using BOXCricket.Models;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Headers;
using NuGet.Common;


namespace BOXCricket.Services
{
    public class ApiClientService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ApiClientService(HttpClient httpClient,IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        #region Common GetAsync 
        public async Task<ApiResponseModel> GetAsync(string endpoint)
        {
            try
            {
                var token = _httpContextAccessor.HttpContext.Request.Cookies["token"];
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.GetAsync(endpoint);
                var responseContent = await response.Content.ReadAsStringAsync();

                ApiResponseModel? apiResponseModel = JsonConvert.DeserializeObject<ApiResponseModel>(responseContent);
                if (apiResponseModel is not null)
                {
                    return apiResponseModel;
                }
                return new ApiResponseModel { StatusCode = (int)response.StatusCode, Message = response.ReasonPhrase };
            }
            catch (Exception ex)
            {
                return new ApiResponseModel { StatusCode = 500, Message = ex.Message };
            }
        }
        #endregion

        #region Common PostAsync
        public async Task<ApiResponseModel> PostAsync(string endpoint, object data)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(endpoint, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                ApiResponseModel? apiResponseModel = JsonConvert.DeserializeObject<ApiResponseModel>(responseContent);

                if (apiResponseModel != null && apiResponseModel.StatusCode != 0)
                {
                    return apiResponseModel;
                }
                return new ApiResponseModel { StatusCode = (int)response.StatusCode, Message = response.ReasonPhrase };
            }
            catch (Exception ex)
            {
                return new ApiResponseModel { StatusCode = 500, Message = ex.Message };
            }
        }
        #endregion

        #region Common PutAsync
        public async Task<ApiResponseModel> PutAsync(string endpoint, object data)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync(endpoint, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                ApiResponseModel? apiResponseModel = JsonConvert.DeserializeObject<ApiResponseModel>(responseContent);

                if (apiResponseModel != null && apiResponseModel.StatusCode != 0)
                {
                    return apiResponseModel;
                }
                return new ApiResponseModel { StatusCode = (int)response.StatusCode, Message = response.ReasonPhrase };
            }
            catch (Exception ex)
            {
                return new ApiResponseModel { StatusCode = 500, Message = ex.Message };
            }
        }
        #endregion


        #region Common DeleteAsync
        public async Task<ApiResponseModel> DeleteAsync(string endpoint)
        {
            try
            {
                var response = await _httpClient.DeleteAsync(endpoint);
                var responseContent = await response.Content.ReadAsStringAsync();

                ApiResponseModel? apiResponseModel = JsonConvert.DeserializeObject<ApiResponseModel>(responseContent);

                if (apiResponseModel != null && apiResponseModel.StatusCode != 0)
                {
                    return apiResponseModel;
                }
                return new ApiResponseModel { StatusCode = (int)response.StatusCode, Message = response.ReasonPhrase };
            }
            catch (Exception ex)
            {
                return new ApiResponseModel { StatusCode = 500, Message = ex.Message };
            }
        }
        #endregion


        #region  Delete Multiple
        public async Task<ApiResponseModel> DeleteMultipleAsync(string endpoint, object data)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri(_httpClient.BaseAddress + endpoint),
                    Content = content
                };
                var response = await _httpClient.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();

                ApiResponseModel? apiResponseModel = JsonConvert.DeserializeObject<ApiResponseModel>(responseContent);

                if (apiResponseModel != null && apiResponseModel.StatusCode != 0)
                {
                    return apiResponseModel;
                }
                return new ApiResponseModel { StatusCode = (int)response.StatusCode, Message = response.ReasonPhrase };
            }
            catch (Exception ex)
            {
                return new ApiResponseModel { StatusCode = 500, Message = ex.Message };
            }
        }
        #endregion

        #region image upload
        public async Task<ApiResponseModel> UploadImage(IFormFile file)
        {
            try
            {
                var content = new MultipartFormDataContent();
                content.Add(new StreamContent(file.OpenReadStream())
                {
                    Headers =
                    {
                        ContentLength = file.Length,
                        ContentType = new MediaTypeHeaderValue(file.ContentType)
                    }
                }, "image", file.FileName);

                var response = await _httpClient.PostAsync("ImageUpload", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                ApiResponseModel? apiResponseModel = JsonConvert.DeserializeObject<ApiResponseModel>(responseContent);

                if (apiResponseModel != null && apiResponseModel.StatusCode != 0)
                {
                    return apiResponseModel;
                }
                return new ApiResponseModel { StatusCode = (int)response.StatusCode, Message = response.ReasonPhrase };
            }
            catch (Exception ex)
            {
                return new ApiResponseModel { StatusCode = 500, Message = ex.Message };
            }
        }
        #endregion
    }
}

