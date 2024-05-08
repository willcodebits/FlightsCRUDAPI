using AutoMapper;
using FlightsCRUDAPI.Models;
using FlightsCRUDAPI.Models.Dtos;

namespace FlightsCRUDAPI.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Flight, FlightApiRequest>();
            CreateMap<FlightApiRequest, Flight>();
        }
    }
}