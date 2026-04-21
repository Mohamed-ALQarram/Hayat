using System.ComponentModel.DataAnnotations;
using Hayat.DAL.Entities.Enums;

namespace Hayat.BLL.DTOs.Doctor
{
    public class UpdateAppointmentStatusRequestDto
    {
        [Required]
        public AppointmentStatus Status { get; set; }

        public string? Reason { get; set; }
    }
}
