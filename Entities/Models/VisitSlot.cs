using System;
using System.Globalization;
using Newtonsoft.Json;

namespace Entities.Models
{
	public class VisitSlot
	{
        public int DoctorId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int VisitId { get; set; }
        public int Id { get; set; }

        // Custom property to parse UTC date/time strings
        [JsonProperty("startTime")]
        public string StartTimeUtcString { get; set; }

        // Custom property to parse UTC date/time strings
        [JsonProperty("endTime")]
        public string EndTimeUtcString { get; set; }

        // Custom method to convert UTC date/time strings to DateTime objects
        public void ParseUtcTimes()
        {
            StartTime = DateTime.ParseExact(StartTimeUtcString, "yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
            EndTime = DateTime.ParseExact(EndTimeUtcString, "yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
        }
    }
}

