using ExternalApiClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Travel.AppData.Entities.Flight;
using Travel.AppData.Entities.Hotel;
using Travel.AppData.Entities.HotelApi.Models;


namespace Travel.AppData
{
    public class TravelService
    {
        private readonly IAccessTokenManager _accessTokenManager;
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public TravelService(IAccessTokenManager accessTokenManager, IConfiguration configuration)
        {
            _accessTokenManager = accessTokenManager;
            _httpClient = new HttpClient();
            _baseUrl = configuration["ApiSettings:BaseUrl"] ?? throw new ArgumentNullException(nameof(configuration), "BaseUrl configuration is missing");
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<FlightOfferResponse> SearchFlightsAsync(FlightSearchRequest search, CancellationToken cancellationToken = default)
        {
            try
            {
                var endpoint = $"/v2/shopping/flight-offers?originLocationCode={search.FlightOrigin}&destinationLocationCode={search.FlightDestination}&departureDate={search.DepartureDate}&adults=1&nonStop=false&max=10";
                using var request = await CreateRequestAsync(HttpMethod.Get, endpoint, cancellationToken);
                using var response = await _httpClient.SendAsync(request, cancellationToken);

                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var output = JsonConvert.DeserializeObject<FlightOfferResponse>(content);
                return output ?? new FlightOfferResponse();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error searching flights: {ex.Message}");
                return new FlightOfferResponse();
            }
        }

        public async Task<List<HotelOfferData>> SearchHotelsAsync(string city, string checkInDate, CancellationToken cancellationToken = default)
        {
            try
            {
                //Get hotels(hotelIds) in specified  city.
                var hotelIds = await GetHotelIDSAsync(city, cancellationToken);
                if (string.IsNullOrEmpty(hotelIds))
                    return new List<HotelOfferData>();

                //Get hotel offers from set of hotels.
                var endpoint = $"/v3/shopping/hotel-offers?hotelIds={hotelIds}&adults=1&checkInDate={checkInDate}&roomQuantity=1&paymentPolicy=NONE&bestRateOnly=true";
                using var request = await CreateRequestAsync(HttpMethod.Get, endpoint, cancellationToken);

                var response = await _httpClient.SendAsync(request, cancellationToken);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var output = JsonConvert.DeserializeObject<HotelOffersResponse>(content);
                return output?.Data ?? new List<HotelOfferData>();
            }

            catch (Exception ex)
            {
                Console.WriteLine($"error searching hotels: {ex.Message}");
                return new List<HotelOfferData>();
            }
        }

        private async Task<string> GetHotelIDSAsync(string city, CancellationToken cancellationToken = default)
        {
            var output = await SearchHotelIDSAsync(city, cancellationToken);
            return string.Join(",", output);
        }

        //Get hotels(hotelIds) in specified  city.
        private async Task<List<string>> SearchHotelIDSAsync(string city, CancellationToken cancellationToken = default)
        {
            try
            {
                var endpoint = $"/v1/reference-data/locations/hotels/by-city?cityCode={city}&radius=5&radiusUnit=KM&hotelSource=ALL";
                using var request = await CreateRequestAsync(HttpMethod.Get, endpoint, cancellationToken);
                using var response = await _httpClient.SendAsync(request, cancellationToken);

                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var output = JsonConvert.DeserializeObject<HotelLocationResponse>(content);
                return output?.Data?.Select(h => h.HotelId).Take(5).ToList() ?? new List<string>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error searching hotels: {ex.Message}");
                return new List<string>();
            }
        }

        //Http Request preparing method.
        private async Task<HttpRequestMessage> CreateRequestAsync(HttpMethod method, string endpoint,
        CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestMessage(method, $"{_baseUrl}{endpoint}");
            var token = await _accessTokenManager.GetAccessTokenAsync(cancellationToken);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return request;
        }
    }
}
