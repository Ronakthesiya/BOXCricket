namespace BOXCricket.Models
{
    public class ApiResponseModel
    {
        public string? Message { get; set; }
        public int StatusCode { get; set; }
        public dynamic? Data { get; set; }
    }
}
