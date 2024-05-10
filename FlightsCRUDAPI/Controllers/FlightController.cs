using FlightsCRUDAPI.Models;
using FlightsCRUDAPI.Models.Dtos;
using FlightsCRUDAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace FlightsCRUDAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FlightController : ControllerBase
    {
        private readonly IFlightService _flightService;
        public FlightController(IFlightService flightService)
        {
            _flightService = flightService;
        }


        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<List<Flight>>>> GetAllFlights()
        {
            var response = await _flightService.GetAllFlights();

            if (response.Data != null)
            {
                return Ok(response);
            }

            if (response.ResponseCode == "404")
            {
                return NotFound(response);
            }

            return new ObjectResult(response);

        }


        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseDto<Flight?>>> GetFlightById(int id)
        {
            var response = await _flightService.GetFlightById(id);

            if (response.Data != null)
            {
                return Ok(response);
            }

            if (response.ResponseCode == "404")
            {
                return NotFound(response);
            }

            return new ObjectResult(response);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponseDto<Flight?>>> CreateFlight(FlightApiRequest apiRequestDto)
        {
            var response = await _flightService.CreateFlight(apiRequestDto);

            if (response.Data != null)
            {
                return Ok(response);
            }

            return new ObjectResult(response);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponseDto<Flight?>>> UpdateFlight(int id, FlightApiRequest apiRequestDto)
        {
            var response = await _flightService.UpdateFlight(id, apiRequestDto);

            if (response.Data != null)
            {
                return Ok(response);
            }

            if (response.ResponseCode.Equals("404"))
            {
                return NotFound(response);
            }

            return new ObjectResult(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponseDto<Flight?>>> DeleteFlight(int id)
        {
            var response = await _flightService.DeleteFlight(id);

            if (response.Data != null)
            {
                return Ok(response);
            }

            if (response.ResponseCode.Equals("404"))
            {
                return NotFound(response);
            }

            return new ObjectResult(response);
        }
    }
}