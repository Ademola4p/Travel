using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ExternalApiClient
{
    public class AccessTokenManager : IAccessTokenManager
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AccessTokenManager> _logger;
        private readonly string _tokenEndpoint;
        private readonly string _apiKey;
        private readonly string _apiSecret;

        private string _accessToken;
        private DateTime _tokenExpirationTime;

        public AccessTokenManager(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<AccessTokenManager> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _tokenEndpoint = configuration["ApiSettings:TokenEndpoint"];
            _apiKey = configuration["ApiSettings:ApiKey"];
            _apiSecret = configuration["ApiSettings:ApiSecret"];
        }

        public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(_accessToken) || DateTime.UtcNow >= _tokenExpirationTime)
            {
                await RefreshAccessTokenAsync(cancellationToken);
            }

            return _accessToken;
        }

        private async Task RefreshAccessTokenAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Refreshing access token...");

            var requestContent = new StringContent(
                 $"grant_type=client_credentials&client_id={_apiKey}&client_secret={_apiSecret}",
                 Encoding.UTF8,
                 "application/x-www-form-urlencoded");

            var response = await _httpClient.PostAsync(_tokenEndpoint, requestContent, cancellationToken);
            response.EnsureSuccessStatusCode();

            var tokenResponse = await JsonSerializer.DeserializeAsync<TokenResponse>(
                await response.Content.ReadAsStreamAsync(), cancellationToken: cancellationToken);

            _accessToken = tokenResponse.AccessToken;
            _tokenExpirationTime = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn);

            _logger.LogDebug($"New access token acquired, valid until {_tokenExpirationTime}");
        }
    }

    public interface IAccessTokenManager
    {
        Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default);
    }

    public class TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
    }
}