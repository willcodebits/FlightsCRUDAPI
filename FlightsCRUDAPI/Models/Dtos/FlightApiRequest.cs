using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightsCRUDAPI.Models.Dtos
{
    public class FlightApiRequest
    {

        public int FlightNumber { get; set; }

        public string AirlineName { get; set; } = "";

        public string DepartureAirportCode { get; set; } = "";

        public string DestinationAirportCode { get; set; } = "";

        public DateTime DepartureDateTime { get; set; }

        public DateTime ArrivalDateTime { get; set; }

        public int PassengerCapacity { get; set; }
    }
}