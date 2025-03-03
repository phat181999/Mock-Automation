using System.Threading.Tasks;
using BookingApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BookingApi.Controllers
{
    [ApiController]
    [Route("api/booking")]
    public class BookingController : ControllerBase
    {
        private readonly BookingService _bookingService;
        private readonly ILogger<BookingController> _logger;

        public BookingController(BookingService bookingService, ILogger<BookingController> logger)
        {
            _bookingService = bookingService;
            _logger = logger;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateBooking([FromBody] BookingRequest request)
        {
            var response = await _bookingService.CreateBookingAsync(request);
            _logger.LogInformation("response: {0}", response);

            if (response == null)
                return BadRequest(new { Message = "Booking creation failed" });

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetBookings()
        {
            var bookings = await _bookingService.GetBookingsAsync();

            if (bookings == null)
                return BadRequest(new { Message = "Failed to fetch bookings" });

            return Ok(bookings);
        }

        // [Authorize] // 🔒 Yêu cầu Authorization
        [HttpDelete("{bookingId}")]
        public async Task<IActionResult> DeleteBooking(int bookingId)
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString(); // Lấy token từ header

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("Missing Authorization header.");
            }

            var result = await _bookingService.DeleteBookingAsync(bookingId, HttpContext);
            if (!result)
            {
                return BadRequest("Failed to delete booking.");
            }
            return Ok("Booking deleted successfully.");
        }

        [Authorize] // 🔒 Yêu cầu Authorization
        [HttpPut("{bookingId}")]
        public async Task<IActionResult> UpdateBooking(int bookingId, [FromBody] BookingRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid booking data.");
            }

            var token = HttpContext.Request.Headers["Authorization"].ToString(); // Lấy token từ header

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("Missing Authorization header.");
            }

            var updatedBooking = await _bookingService.UpdateBookingAsync(bookingId, request, HttpContext);

            if (updatedBooking == null)
            {
                return BadRequest($"Failed to update booking with ID {bookingId}.");
            }

            return Ok(updatedBooking);
        }
    }
}
