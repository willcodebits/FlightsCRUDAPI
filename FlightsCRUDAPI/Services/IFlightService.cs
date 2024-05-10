using FlightsCRUDAPI.Models;
using FlightsCRUDAPI.Models.Dtos;

namespace FlightsCRUDAPI.Services
{
    public interface IFlightService
    {
        Task<ApiResponseDto<List<Flight>?>> GetAllFlights();
        Task<ApiResponseDto<Flight?>> GetFlightById(int id);
        Task<ApiResponseDto<Flight?>> CreateFlight(FlightApiRequest apiRequestDto);

        Task<ApiResponseDto<Flight?>> UpdateFlight(int id, FlightApiRequest flightToUpdateDto);
        Task<ApiResponseDto<Flight?>> DeleteFlight(int id);
    }
}