using Hayat.DAL.Entities.Enums;

namespace Hayat.BLL.DTOs.Doctor
{
    public class UpdateAppointmentStatusResponseDto
    {
        public int AppointmentId { get; set; }
        public AppointmentStatus Status { get; set; }
        public bool Updated { get; set; }
    }
}
