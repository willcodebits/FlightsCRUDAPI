using AutoMapper;
using FlightsCRUDAPI.Data;
using FlightsCRUDAPI.Models;
using FlightsCRUDAPI.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace FlightsCRUDAPI.Services
{
    public class FlightService : IFlightService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public FlightService(ApplicationDbContext dbContext, IMapper mapper)
        {
            _mapper = mapper;

            _dbContext = dbContext;
        }

        public async Task<ApiResponseDto<List<Flight>?>> GetAllFlights()
        {

            try
            {
                var flights = await _dbContext.Flights.ToListAsync();
                return new ApiResponseDto<List<Flight>?>
                {
                    RequestFailed = false,
                    Data = flights,
                    ResponseCode = "200"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<List<Flight>?>
                {
                    RequestFailed = true,
                    Data = null,
                    ResponseCode = "500",
                    ErrorMessage = $"An error occurred retriving the flights from the database: {ex.Message}"
                };

            }

        }

        public async Task<ApiResponseDto<Flight?>> GetFlightById(int id)
        {
            try
            {
                var flight = await _dbContext.Flights.FindAsync(id);
                if (flight is null)
                {
                    return new ApiResponseDto<Flight?>
                    {
                        RequestFailed = true,
                        Data = null,
                        ResponseCode = "404",
                        ErrorMessage = $"Flight with id: {id} was not found."

                    };
                }
                return new ApiResponseDto<Flight?>
                {
                    RequestFailed = false,
                    Data = flight,
                    ResponseCode = "200"

                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<Flight?>
                {
                    RequestFailed = true,
                    Data = null,
                    ResponseCode = "500",
                    ErrorMessage = $"An error occurred retriving the flight from the database: {ex.Message}"
                };
            }

        }

        public async Task<ApiResponseDto<Flight?>> CreateFlight(FlightApiRequest apiRequestDto)
        {
            // This mapping will create a new Flight object from the dto
            Flight newFlight = _mapper.Map<Flight>(apiRequestDto);

            try
            {
                await _dbContext.Flights.AddAsync(newFlight);

                await _dbContext.SaveChangesAsync();

                return new ApiResponseDto<Flight?>
                {
                    RequestFailed = false,
                    Data = newFlight,
                    ResponseCode = "201"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<Flight?>
                {
                    RequestFailed = true,
                    Data = null,
                    ResponseCode = "500",
                    ErrorMessage = $"An error occurred saving the flight on the database: {ex.Message}"
                };
            }

        }

        public async Task<ApiResponseDto<Flight?>> UpdateFlight(int id, FlightApiRequest flightToUpdateDto)
        {
            Flight? dbFlight;

            try
            {
                /*
                Here I show that firstordefault async can look for the id as well but 
                preferably you will use FindAsync if you will search by primary key
                and use FirstOrDefaultAsync if you will search by other property instead
                of primary key
                */
                dbFlight = await _dbContext.Flights.FirstOrDefaultAsync(f => f.Id == id);

                if (dbFlight == null)
                {
                    return new ApiResponseDto<Flight?>
                    {
                        RequestFailed = true,
                        Data = null,
                        ResponseCode = "404",
                        ErrorMessage = $"Flight with id: {id} was not found."

                    };
                }
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<Flight?>
                {
                    RequestFailed = true,
                    Data = null,
                    ResponseCode = "500",
                    ErrorMessage = $"An error occurred saving the flight on the database: {ex.Message}"
                };
            }

            try
            {
                /*
                This time we don't use the generic as we want to 
                make sure we use the dbFlight object and not create
                a new flight object
                */
                _mapper.Map(flightToUpdateDto, dbFlight);

                await _dbContext.SaveChangesAsync();

                return new ApiResponseDto<Flight?>
                {
                    RequestFailed = false,
                    Data = dbFlight,
                    ResponseCode = "200"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<Flight?>
                {
                    RequestFailed = true,
                    Data = null,
                    ResponseCode = "500",
                    ErrorMessage = $"An error occurred updating the flight on the database: {ex.Message}"
                };
            }



        }

        public async Task<ApiResponseDto<Flight?>> DeleteFlight(int id)
        {
            Flight? dbFlight;

            try
            {
                dbFlight = await _dbContext.Flights.FirstOrDefaultAsync(f => f.Id == id);

                if (dbFlight is null)
                {
                    return new ApiResponseDto<Flight?>
                    {
                        RequestFailed = true,
                        Data = null,
                        ResponseCode = "404",
                        ErrorMessage = $"Flight with id: {id} was not found."

                    };
                }
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<Flight?>
                {
                    RequestFailed = true,
                    Data = null,
                    ResponseCode = "500",
                    ErrorMessage = $"An error occurred retriving the flight on the database: {ex.Message}"
                };

            }

            try
            {
                _dbContext.Flights.Remove(dbFlight);

                await _dbContext.SaveChangesAsync();

                return new ApiResponseDto<Flight?>
                {
                    RequestFailed = false,
                    Data = dbFlight,
                    ResponseCode = "200"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<Flight?>
                {
                    RequestFailed = true,
                    Data = null,
                    ResponseCode = "500",
                    ErrorMessage = $"An error occurred deleting the flight on the database: {ex.Message}"
                };
            }

        }

    }
}