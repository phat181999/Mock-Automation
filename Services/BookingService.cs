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

        public async Task<UpdateBookingResponse?> UpdateBookingAsync(int bookingId, BookingRequest request, HttpContext httpContext)
        {
            var jsonPayload = JsonSerializer.Serialize(request);
            _logger.LogInformation("Updating booking {0} with data: {1}", bookingId, jsonPayload);

            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Clear();
            // Lấy token từ header của request
            if (!httpContext.Request.Headers.TryGetValue("Authorization", out var token))
            {
                _logger.LogError("Authorization token is missing!");
                return null;
            }

            _logger.LogInformation("Using token: {0}", token);

            // Thêm token vào header
            _httpClient.DefaultRequestHeaders.Add("Cookie", $"token={token}");
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            var response = await _httpClient.PutAsync($"https://restful-booker.herokuapp.com/booking/{bookingId}", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("Response: {0}", responseBody);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to update booking {0}! Status Code: {1}, Response: {2}", bookingId, response.StatusCode, responseBody);
                return null;
            }

            _logger.LogInformation("Booking {0} updated successfully!", bookingId);
            return JsonSerializer.Deserialize<UpdateBookingResponse>(responseBody);
        }

        public async Task<bool> DeleteBookingAsync(int bookingId, HttpContext httpContext)
        {
            _logger.LogInformation("Deleting booking {0}...", bookingId);

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            // Lấy token từ header của request
            if (!httpContext.Request.Headers.TryGetValue("Authorization", out var token))
            {
                _logger.LogError("Authorization token is missing!");
                return false;
            }

            _logger.LogInformation("Using token: {0}", token);

            // Thêm token vào header
            _httpClient.DefaultRequestHeaders.Add("Cookie", $"token={token}");

            var response = await _httpClient.DeleteAsync($"https://restful-booker.herokuapp.com/booking/{bookingId}");
            var responseBody = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("Response: {0}", responseBody);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to delete booking {0}! Status Code: {1}, Response: {2}", bookingId, response.StatusCode, responseBody);
                return false;
            }

            _logger.LogInformation("Booking {0} deleted successfully!", bookingId);
            return true;
        }
    }

    public class BookingRequest
    {
        public string firstname { get; set; } = string.Empty;
        public string lastname { get; set; } = string.Empty;
        public int? totalprice { get; set; } 
        public bool depositpaid { get; set; }
        public BookingDates bookingdates { get; set; } = new BookingDates();
        public string additionalneeds { get; set; } = string.Empty;
    }

    public class BookingDates
    {
        public string checkin { get; set; } = string.Empty;
        public string checkout { get; set; } = string.Empty;
    }

    public class BookingResponse
    {
        public int bookingid { get; set; }
        public BookingRequest booking { get; set; } = new BookingRequest();
    }

    public class UpdateBookingResponse
    {
        public string firstname { get; set; } = string.Empty;
        public string lastname { get; set; } = string.Empty;
        public int? totalprice { get; set; } 
        public bool depositpaid { get; set; }
        public BookingDates bookingdates { get; set; } = new BookingDates();
        public string additionalneeds { get; set; } = string.Empty;
    }

    // public class BookingDates
    // {
    //     public string checkin { get; set; } = string.Empty;
    //     public string checkout { get; set; } = string.Empty;
    // }


}
