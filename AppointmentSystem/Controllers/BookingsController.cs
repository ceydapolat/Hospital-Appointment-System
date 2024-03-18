using System;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;

namespace AppointmentSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }


        [HttpPost("book")]
        public async Task<IActionResult> BookVisit(BookingRequest request)
        {
            var result = await _bookingService.BookVisitAsync(request);

            if (!result.Status)
                return BadRequest(new { message = "NO_SLOT_FOUND" });

            return Ok(result);
        }

        [HttpPost("cancel")]
        public async Task<IActionResult> CancelVisit(CancellationRequest request)
        {
            if (request.BookingId == 0)
                return new JsonResult(new { status = false });

            var result = await _bookingService.CancelVisitAsync(request);
            return Ok(result);
        }
    }
}

