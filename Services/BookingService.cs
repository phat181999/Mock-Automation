using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace BookingApi.Services
{
    public class BookingService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<BookingService> _logger;

        public BookingService(HttpClient httpClient, ILogger<BookingService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<BookingResponse?> CreateBookingAsync(BookingRequest request)
        {
            var jsonPayload = JsonSerializer.Serialize(request);
            _logger.LogInformation("Sending booking request: {0}", jsonPayload);

            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            // Đặt Header
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                

            var response = await _httpClient.PostAsync("https://restful-booker.herokuapp.com/booking", content);
             _logger.LogInformation("Sending booking request: {0}", response.StatusCode, response.Content);
            var responseBody = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("Response Body: {0}", responseBody);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            _logger.LogInformation("Booking successful!");

            return JsonSerializer.Deserialize<BookingResponse>(responseBody);
        }

        public async Task<string?> GetBookingsAsync()
        {
            try
            {
                _logger.LogInformation("Fetching booking list...");

                var response = await _httpClient.GetAsync("https://restful-booker.herokuapp.com/booking");
                var responseBody = await response.Content.ReadAsStringAsync();

                _logger.LogError("Hello Response Body: {0}", responseBody);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to fetch bookings! Status Code: {0}, Response: {1}", response.StatusCode, responseBody);
                    return null;
                }

                _logger.LogInformation("Fetched bookings successfully!");
                return responseBody;

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to fetch bookings! Status Code: {0}, Response: {1}", response.StatusCode, responseBody);
                    return null;
                }

                _logger.LogInformation("Fetched bookings successfully!");
                return responseBody;
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception occurred while fetching bookings: {0}", ex.Message);
                return null;
            }
        }
    }

    public class BookingRequest
    {
        public string Firstname { get; set; } = string.Empty;
        public string Lastname { get; set; } = string.Empty;
        public int? TotalPrice { get; set; }  // Hỗ trợ giá trị null
        public bool DepositPaid { get; set; }
        public BookingDates BookingDates { get; set; } = new BookingDates();
        public string AdditionalNeeds { get; set; } = string.Empty;
    }

    public class BookingDates
    {
        public string Checkin { get; set; } = string.Empty;
        public string Checkout { get; set; } = string.Empty;
    }

    public class BookingResponse
    {
        public int BookingId { get; set; }
        public BookingRequest Booking { get; set; } = new BookingRequest();
    }
}
