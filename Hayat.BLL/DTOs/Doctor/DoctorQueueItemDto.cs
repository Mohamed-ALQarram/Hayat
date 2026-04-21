using Hayat.DAL.Entities.Enums;
namespace Hayat.BLL.DTOs.Doctor
{
    public class DoctorQueueItemDto
    {
        public int AppointmentId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public Gender Gender { get; set; }
        public int Age { get; set; } 
        public string Phone { get; set; } = string.Empty;
        public int ClinicId { get; set; }
        public string ClinicName { get; set; } = string.Empty;
    }
}
