using System;
using System.Collections.Generic;
using System.Text;

namespace Travel.AppData.Entities.Hotel
{
    public class HotelOffersResponse
    {
        public List<HotelOfferData> Data { get; set; }
    }

    public class HotelOfferData
    {
        public string Type { get; set; }
        public Hotel Hotel { get; set; }
        public bool Available { get; set; }
        public List<Offer> Offers { get; set; }
        public string Self { get; set; }
    }

    public class Hotel
    {
        public string Type { get; set; }
        public string HotelId { get; set; }
        public string ChainCode { get; set; }
        public string DupeId { get; set; }
        public string Name { get; set; }
        public string CityCode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class Offer
    {
        public string Id { get; set; }
        public string CheckInDate { get; set; }
        public string CheckOutDate { get; set; }
        public string RateCode { get; set; }
        public RateFamilyEstimated RateFamilyEstimated { get; set; }
        public Room Room { get; set; }
        public Guests Guests { get; set; }
        public Price Price { get; set; }
        public Policies Policies { get; set; }
        public string Self { get; set; }
    }

    public class RateFamilyEstimated
    {
        public string Code { get; set; }
        public string Type { get; set; }
    }

    public class Room
    {
        public string Type { get; set; }
        public TypeEstimated TypeEstimated { get; set; }
        public Description Description { get; set; }
    }

    public class TypeEstimated
    {
        public string Category { get; set; }
        public int Beds { get; set; }
        public string BedType { get; set; }
    }

    public class Description
    {
        public string Text { get; set; }
        public string Lang { get; set; }
    }

    public class Guests
    {
        public int Adults { get; set; }
    }

    public class Price
    {
        public string Currency { get; set; }
        public string Base { get; set; }
        public string Total { get; set; }
        public PriceVariations Variations { get; set; }
    }

    public class PriceVariations
    {
        public Average Average { get; set; }
        public List<Change> Changes { get; set; }
    }

    public class Average
    {
        public string Base { get; set; }
    }

    public class Change
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Total { get; set; }
    }

    public class Policies
    {
        public string PaymentType { get; set; }
        public Cancellation Cancellation { get; set; }
    }

    public class Cancellation
    {
        public Description Description { get; set; }
        public string Type { get; set; }
    }
}
