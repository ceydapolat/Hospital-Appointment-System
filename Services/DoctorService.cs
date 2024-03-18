using System.Net;
using Entities.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Services.Contracts;

namespace Services
{
    public class DoctorService : IDoctorService
    {
        private readonly HttpClient _httpClient;
        private readonly ILoggerService _logger;
        private IEnumerable<Doctor> _doctors; // Cache for doctors

        public DoctorService(HttpClient httpClient, ILoggerService logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IEnumerable<Doctor>> GetDoctorsAsync()
        {
            try
            {
                if (_doctors is null)
                {
                    var response = await _httpClient.GetAsync("https://fe8f4f5e-f5c2-48b6-974c-097f4cec3de0.mock.pstmn.io/fetchDoctors");

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        var responseObject = JObject.Parse(jsonResponse);
                        _doctors = responseObject["data"].ToObject<List<Doctor>>();
                    }
                    else
                    {
                        _logger.LogError($"Failed to fetch doctors. Status code: {response.StatusCode}");
                    }
                }

                return _doctors;
            }
            catch
            {
                _logger.LogError("Failed to fetch doctors");
            }


            return null;
        }

        public async Task<List<VisitSlot>> GetAvailableSlotsAsync(int? doctorId)
        {
            var doctors = await GetDoctorsAsync();

            if (doctors is not null)
            {
                var docExists = doctors.Any(d => d.DoctorId == doctorId);

                if (docExists is false)
                {
                    _logger.LogError($"No doctor with this doctor ID could be found.");

                    return null;
                }
            }

            var response = await _httpClient.GetAsync($"https://fe8f4f5e-f5c2-48b6-974c-097f4cec3de0.mock.pstmn.io/fetchSchedules?doctorId={doctorId}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var slotsData = JsonConvert.DeserializeObject<SlotData>(content);

                if (slotsData != null && slotsData.Data != null)
                {
                    List<VisitSlot> visitSlots = new List<VisitSlot>();

                    foreach (var slot in slotsData.Data)
                    {
                        var visitSlot = new VisitSlot
                        {
                            DoctorId = slot.DoctorId,
                            StartTimeUtcString = slot.StartTimeUtcString,
                            EndTimeUtcString = slot.EndTimeUtcString,
                            VisitId = slot.VisitId,
                            Id = slot.Id
                        };

                        visitSlot.ParseUtcTimes(); // Parse UTC times to DateTime objects

                        visitSlots.Add(visitSlot);
                    }

                    return visitSlots;
                }
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return new List<VisitSlot>();
            }

            return null;
        }

        private class SlotData
        {
            public List<VisitSlot> Data { get; set; }
        }

        public async Task<int> GetDoctorIdByDoctorNameAsync(string doctorName)
        {
            var doctors = await GetDoctorsAsync();
            var doctor = doctors.FirstOrDefault(d => d.Name.Equals(doctorName));

            if (doctor is null)
                return default;

            return doctor.DoctorId;
        }

        public async Task<(int? HospitalId, int? BranchId)> GetDoctorInfoByDoctorIdAsync(int doctorId)
        {
            var doctors = await GetDoctorsAsync();
            var doctor = doctors.FirstOrDefault(d => d.DoctorId == doctorId);

            return (doctor?.HospitalId, (int)doctor?.BranchId);
        }

        public async Task ExportTurkishDoctorsToCsvAsync()
        {
            var doctors = await GetDoctorsAsync();

            // Filter doctors who are Turkish and convert gender values
            var turkishDoctors = doctors
                .Where(d => d.Nationality == "TUR")
                .Select(d => new Doctor //New instance created to avoid altering original data
                {
                    Name = d.Name,
                    Gender = GenderHelper.ConvertToTurkish(d.Gender),
                    HospitalName = d.HospitalName,
                    HospitalId = d.HospitalId,
                    SpecialtyId = d.SpecialtyId,
                    BranchId = d.BranchId,
                    Nationality = d.Nationality,
                    DoctorId = d.DoctorId,
                    CreatedAt = d.CreatedAt
                })
                .ToList();


            var filePath = "DoctorsExport.csv";

            CsvHelper.ExportToCsv(turkishDoctors, filePath);
        }


    }
}

