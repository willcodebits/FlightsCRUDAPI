namespace FlightsCRUDAPI.Models.Dtos
{
    public class ApiResponseDto<T>
    {
        public bool RequestFailed { get; set; } = false;

        public string ResponseCode { get; set; } = string.Empty;

        public string ErrorMessage { get; set; } = string.Empty;
        public T? Data { get; set; }
    }
}