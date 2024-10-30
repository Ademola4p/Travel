using System;
using System.Collections.Generic;
using System.Text;

namespace Travel.AppData.Entities.Flight
{
    public class FlightSearchRequest
    {
        public string FlightOrigin { get; set; }
        public string FlightDestination { get; set; }
        public string DepartureDate { get; set; }

    }
}
