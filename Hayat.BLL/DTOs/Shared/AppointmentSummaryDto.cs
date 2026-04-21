namespace Hayat.BLL.DTOs.Shared
{
    public class AppointmentSummaryDto
    {
        public int AppointmentId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public int ClinicId { get; set; }
        public string ClinicName { get; set; } = string.Empty;
        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public Guid? DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
    }
}
