﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Travel.AppData.Entities
{
    using System.Collections.Generic;

    namespace HotelApi.Models
    {
        public class HotelLocationResponse
        {
            public List<HotelData> Data { get; set; }
            public Meta Meta { get; set; }
        }

        public class HotelData
        {
            public string ChainCode { get; set; }
            public string IataCode { get; set; }
            public long DupeId { get; set; }
            public string Name { get; set; }
            public string HotelId { get; set; }
            public GeoCode GeoCode { get; set; }
            public Address Address { get; set; }
            public Distance Distance { get; set; }
        }

        public class GeoCode
        {
            public double Latitude { get; set; }
            public double Longitude { get; set; }
        }

        public class Address
        {
            public string CountryCode { get; set; }
        }

        public class Distance
        {
            public double Value { get; set; }
            public string Unit { get; set; }
        }

        public class Meta
        {
            public int Count { get; set; }
            public Links Links { get; set; }
        }

        public class Links
        {
            public string Self { get; set; }
        }
    }
}
