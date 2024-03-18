using Entities.Models;
using Newtonsoft.Json;
using Services.Contracts;

namespace Services
{
    public class BookingService : IBookingService
    {
        private readonly HttpClient _httpClient;
        private readonly IDoctorService _doctorService;
        private readonly ILoggerService _logger;

        public BookingService(HttpClient httpClient, IDoctorService doctorService, ILoggerService logger)
        {
            _httpClient = httpClient;
            _doctorService = doctorService;
            _logger = logger;
        }

        public async Task<BookingResponse> BookVisitAsync(BookingRequest request)
        {
            var doctorId = await _doctorService.GetDoctorIdByDoctorNameAsync(request.DoctorName);

            if (doctorId == default)
                return new BookingResponse { Status = false };

            var doctorInfo = await _doctorService.GetDoctorInfoByDoctorIdAsync(doctorId);

            if (doctorInfo.HospitalId == null || doctorInfo.BranchId == null)
                return new BookingResponse { Status = false };

            var formattedDate = FormatDate(request.Date);
            var startTime = GetFormattedDateTime(formattedDate, request.StartTime);
            var endTime = GetFormattedDateTime(formattedDate, request.EndTime);

            var slot = await GetAvailableSlotAsync(doctorId, startTime, endTime);

            if (slot is null)
                return new BookingResponse { Status = false };

            var patientName = ExtractPatientName(request.PatientName);
            var patientSurname = ExtractPatientSurname(request.PatientName);

            // Construct the URL based on the request parameters
            var baseUrl = "https://fe8f4f5e-f5c2-48b6-974c-097f4cec3de0.mock.pstmn.io/BookVisit";
            var queryParams = new Dictionary<string, string>
                {
                    { "VisitId", slot.VisitId.ToString() },
                    { "startTime", request.StartTime },
                    { "endTime", request.EndTime },
                    { "date", request.Date },
                    { "PatientName", patientName },
                    { "PatientSurname", patientSurname },
                    { "hospitalId", doctorInfo.HospitalId.ToString() },
                    { "doctorId", doctorId.ToString() },
                    { "branchId", doctorInfo.BranchId.ToString() }
                };

            var queryString = string.Join("&", queryParams.Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));
            var fullUrl = $"{baseUrl}?{queryString}";

            var response = await _httpClient.PostAsync(fullUrl, null);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var bookingResponse = JsonConvert.DeserializeObject<BookingResponse>(content);

                _logger.LogInfo("Booking request processed successfully.");
                return bookingResponse;
            }

            _logger.LogError($"An error occurred while processing booking request");

            return null;
        }

        public async Task<CancellationResponse> CancelVisitAsync(CancellationRequest request)
        {
            // Construct the URL based on the request parameters
            var baseUrl = "https://fe8f4f5e-f5c2-48b6-974c-097f4cec3de0.mock.pstmn.io/bookVisit";
            var queryParams = new Dictionary<string, string>
            {
                { "BookingID", request.BookingId.ToString() }
            };

            var queryString = string.Join("&", queryParams.Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));
            var fullUrl = $"{baseUrl}?{queryString}";

            var response = await _httpClient.PostAsync(fullUrl, null);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var cancellationResponse = JsonConvert.DeserializeObject<CancellationResponse>(content);

                _logger.LogInfo("Cancellation request processed successfully.");

                return cancellationResponse;
            }

            _logger.LogError($"An error occurred while processing cancellation request");

            return null;
        }

        private string ExtractPatientName(string fullName)
        {
            return fullName.Split(' ')[0];
        }

        private string ExtractPatientSurname(string fullName)
        {
            var nameParts = fullName.Split(' ');
            return nameParts.Length > 1 ? nameParts[1] : "";
        }

        private async Task<VisitSlot> GetAvailableSlotAsync(int doctorId, string startTime, string endTime)
        {
            var slots = await _doctorService.GetAvailableSlotsAsync(doctorId);

            return slots.FirstOrDefault(s =>
                s.StartTimeUtcString == startTime &&
                s.EndTimeUtcString == endTime &&
                s.DoctorId == doctorId);
        }

        private string GetFormattedDateTime(string date, string time)
        {
            return $"{date}T{time}:00.000Z";
        }

        private string FormatDate(string date)
        {
            return DateTime.ParseExact(date, "dd/MM/yyyy", null).ToString("yyyy-MM-dd");
        }

    }
}

