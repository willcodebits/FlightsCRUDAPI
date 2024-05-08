namespace FlightsCRUDAPI.Models
{
    public class Flight
    {
        public int Id { get; set; }

        public int FlightNumber { get; set; }

        public string AirlineName { get; set; } = "";

        public string DepartureAirportCode { get; set; } = "";

        public string DestinationAirportCode { get; set; } = "";

        public DateTime DepartureDateTime { get; set; }

        public DateTime ArrivalDateTime { get; set; }

        public int PassengerCapacity { get; set; }
    }
}