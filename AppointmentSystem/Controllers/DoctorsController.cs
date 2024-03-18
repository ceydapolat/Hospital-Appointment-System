using Microsoft.AspNetCore.Mvc;
using Services.Contracts;

namespace AppointmentSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorsController : ControllerBase
    {
        private readonly IDoctorService _doctorService;

        public DoctorsController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDoctorsAsync()
        {
            var doctors = await _doctorService.GetDoctorsAsync();
            return Ok(doctors);
        }

        [HttpGet("doctors/{doctorId}")]
        public async Task<IActionResult> GetAvailableSlots(int doctorId)
        {
            try //I was going to delete try/catch blocks after adding Global Exception Handling, but I received "Too Many Requests" error. Since I will not be able to test them, I left them as they are.
            {
                var slots = await _doctorService.GetAvailableSlotsAsync(doctorId);

                if (slots is null)
                    return NotFound(new { message = "NO_DOCTOR_FOUND" });

                if (!slots.Any())
                    return NotFound(new { message = "NO_SLOT_FOUND" });

                return Ok(slots);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet("export-to-csv")]
        public async Task<IActionResult> ExportToCsv()
        {
            try
            {
                await _doctorService.ExportTurkishDoctorsToCsvAsync();
                return Ok("Data exported to CSV successfully");
            }
            catch
            {
                return StatusCode(500, new { error = "An error occurred while exporting data to CSV." });
            }
        }
    }
}

