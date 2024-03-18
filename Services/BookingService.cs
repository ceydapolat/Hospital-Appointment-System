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

            if(doctorId == default)
                return new BookingResponse { Status = false };

            var doctorInfo = await _doctorService.GetDoctorInfoByDoctorIdAsync(doctorId);

            if (doctorInfo.HospitalId == null || doctorInfo.BranchId == null)
                return new BookingResponse { Status = false };

            var formattedDate = DateTime.ParseExact(request.Date, "dd/MM/yyyy", null).ToString("yyyy-MM-dd");

            var startTime = formattedDate + "T" + request.StartTime + ":00.000Z";
            var endTime = formattedDate + "T" + request.EndTime + ":00.000Z";

            var slots = await _doctorService.GetAvailableSlotsAsync(doctorId);
            var slot = slots.Where(s =>
                s.StartTimeUtcString == startTime &&
                s.EndTimeUtcString == endTime &&
                s.DoctorId == doctorId).FirstOrDefault();

            if (slot is null)
                return new BookingResponse { Status = false };

            // Split patient name into PatientName and PatientSurname
            string[] nameParts = request.PatientName.Split(' ');
            string patientName = nameParts[0];
            string patientSurname = nameParts.Length > 1 ? nameParts[1] : "";

            // Construct the URL based on the request parameters
            var baseUrl = "https://3aff8cc7-91f8-4577-bef3-e566d6c41d74.mock.pstmn.io/BookVisit";
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
                return bookingResponse;
            }
            
            return null;
        }

        public async Task<CancellationResponse> CancelVisitAsync(CancellationRequest request)
        {
            // Construct the URL based on the request parameters
            var baseUrl = "https://3aff8cc7-91f8-4577-bef3-e566d6c41d74.mock.pstmn.io/bookVisit";
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
                return cancellationResponse;
            }
            return null;
        }

    }
}

