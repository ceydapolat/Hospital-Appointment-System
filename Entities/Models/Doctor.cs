using System;
namespace Entities.Models
{
    public class Doctor
    {
        public string Name { get; set; }
        public string Gender { get; set; }
        public string HospitalName { get; set; }
        public int HospitalId { get; set; }
        public int SpecialtyId { get; set; }
        public double BranchId { get; set; }
        public string Nationality { get; set; }
        public int DoctorId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

