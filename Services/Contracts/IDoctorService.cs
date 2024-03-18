using System;
using System.Numerics;
using Entities.Models;

namespace Services.Contracts
{
	public interface IDoctorService
	{
        Task<IEnumerable<Doctor>> GetDoctorsAsync();
        Task<List<VisitSlot>> GetAvailableSlotsAsync(int? doctorId);
        Task<int> GetDoctorIdByDoctorNameAsync(string doctorName);
        Task<(int? HospitalId, int? BranchId)> GetDoctorInfoByDoctorIdAsync(int doctorId);
        Task ExportTurkishDoctorsToCsvAsync();
    }
}

