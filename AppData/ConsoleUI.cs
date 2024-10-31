using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Travel.AppData.Entities.Flight;
using Travel.AppData.Entities.Hotel;

namespace Travel.AppData
{
    //Class that represents user interface for the app.
    public class ConsoleUI
    {
        private readonly TravelService _apiService;

        public ConsoleUI(TravelService apiService)
        {
            _apiService = apiService;
        }

        //Main UI method. UI starts here.
        public async Task RunAsync()
        {

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Travel Booking System ===");
                Console.WriteLine("1. Search Flights");
                Console.WriteLine("2. Search Hotels");
                Console.WriteLine("3. Exit");
                Console.Write("\nSelect an option: ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await SearchFlightsUI();
                        break;
                    case "2":
                        await SearchHotelsUI();
                        break;
                    case "3":
                        return;
                    default:
                        Console.WriteLine("Invalid option. Press any key to continue...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        //Search flights user interface.
        private async Task SearchFlightsUI()
        {
            var search = new FlightSearchRequest();
            Console.Clear();
            Console.WriteLine("=== Flight Search ===");

            Console.Write("Enter origin city (e.g., NYC): ");
            search.FlightOrigin = Console.ReadLine();

            Console.Write("Enter destination city (e.g., LAX): ");
            search.FlightDestination = Console.ReadLine();

            Console.Write("Enter travel date (YYYY-MM-DD): ");
            search.DepartureDate = Console.ReadLine();
            if (DateTime.TryParse(search.DepartureDate, out DateTime date))
            {
                var flights = await _apiService.SearchFlightsAsync(search);
                DisplayFlights(flights.Data, flights.Dictionaries);
            }
            else
            {
                Console.WriteLine("Invalid date format.");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        //Search hotels user interface.
        private async Task SearchHotelsUI()
        {
            Console.Clear();
            Console.WriteLine("=== Hotel Search ===");

            Console.Write("Enter city name (e.g PAR): ");
            var city = Console.ReadLine();

            Console.Write("Enter check-in date (YYYY-MM-DD): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime checkIn))
            {
                Console.WriteLine("Invalid check-in date format.");
                return;
            }

            var hotels = await _apiService.SearchHotelsAsync(city, checkIn.ToString("yyyy-MM-dd"));
            DisplayHotels(hotels);

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        // Tabular results display for flights.
        private void DisplayFlights(List<FlightOffer> flights, Dictionaries dictionaries)
        {
            Console.WriteLine("\nFound {0} flights:", flights.Count);
            Console.WriteLine(new string('-', 110));
            Console.WriteLine("  {0,-15} {1,-10} {2,-30} {3,-30} {4,8}",
                "Flight", "Airline", "Departure", "Arrival", "Price");
            Console.WriteLine(new string('-', 110));

            foreach (var flight in flights)
            {
                Console.WriteLine("  {0,-15} {1,-10} {2,-30} {3,-30} ${4,7:F2}",
                   GetDictionaryValue(dictionaries.Carriers, flight.Itineraries[0].Segments[0].CarrierCode),
                    flight.Itineraries[0].Segments[0].Number,
                    $"{flight.Itineraries[0].Segments[0].Departure.IataCode} ({flight.Itineraries[0].Segments[0].Departure.At.ToString()})",
                    $"{flight.Itineraries[0].Segments[0].Arrival.IataCode} ({flight.Itineraries[0].Segments[0].Arrival.At.ToString()})",
                    flight.Price.GrandTotal);
            }
        }

        //Dictionary search helper by key.
        private string GetDictionaryValue(Dictionary<string, string> dictionaries, string key)
        {
            if (!dictionaries.TryGetValue(key, out string value))
            {
                return key;
            }
            return value;
        }

        // Tabular results display for hotels.
        private void DisplayHotels(List<HotelOfferData> hotels)
        {
            Console.WriteLine("\nFound {0} hotels:", hotels.Count);
            Console.WriteLine(new string('-', 100));
            Console.WriteLine("{0,-30} {1,-6} {2,-15} {3,-15} {4,-10}",
                "Name", "Availability", "CheckIn Date", "Room Type", "Price/Night");
            Console.WriteLine(new string('-', 100));

            foreach (var hotel in hotels)
            {
                foreach (var offer in hotel.Offers)
                {
                    Console.WriteLine("{0,-30} {1,-6} {2,-15} {3,-15} {4,9:F2}",
                        TruncateString(hotel.Hotel.Name, 30),
                        hotel.Available,
                        offer.CheckInDate,
                        offer.Room.TypeEstimated.Category,
                        $"{offer.Price.Currency} {offer.Price.Total}");
                }
            }
        }

        //string truncating helper for long
        private static string TruncateString(string input, int maxLength)
        {
            return input.Length <= maxLength ? input : input[..(maxLength - 3)] + "...";
        }
    }
}

