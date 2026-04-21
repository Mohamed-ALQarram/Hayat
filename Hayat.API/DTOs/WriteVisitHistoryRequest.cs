using Hayat.BLL.DTOs.Doctor;

namespace Hayat.API.DTOs
{
    public class WriteVisitHistoryRequest
    {
        public string PatientComplaint { get; set; } = string.Empty;
        public string Diagnosis { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public List<PrescriptionDto>? prescriptions { get; set; }
    }
}
