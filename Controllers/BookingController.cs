using System.Threading.Tasks;
using BookingApi.Services;
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

        /// <summary>
        /// API POST để tạo booking.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] BookingRequest request)
        {
            var response = await _bookingService.CreateBookingAsync(request);
            if (response == null)
                return BadRequest(new { Message = "Booking creation failed" });

            return Ok(response);
        }

        /// <summary>
        /// API GET để lấy danh sách bookings.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetBookings()
        {
            // var response = await _bookingService.GetBookingAsync();
            var bookings = await _bookingService.GetBookingsAsync();

            if (bookings == null)
                return BadRequest(new { Message = "Failed to fetch bookings" });

            return Ok(bookings);
        }
    }
}
