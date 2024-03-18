using System;
namespace Entities.Models
{
	public class BookingRequest
	{
        public string? PatientName { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
        public string? Date { get; set; }
        public string DoctorName { get; set; }
    }
}

