using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace BookingApi.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AuthService> _logger;

        public AuthService(HttpClient httpClient, ILogger<AuthService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<string> LoginAsync(string username, string password)
        {
            var payload = new { username, password };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://restful-booker.herokuapp.com/auth", content);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Login failed! Status Code: {0}", response.StatusCode);
                return null;
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<AuthResponse>(responseBody);

            return result?.token;
        }
    }

    public class AuthResponse
    {
        public string? token { get; set; }
    }
}
