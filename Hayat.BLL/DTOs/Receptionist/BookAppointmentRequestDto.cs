using System.ComponentModel.DataAnnotations;

namespace Hayat.BLL.DTOs.Receptionist
{
    public class BookAppointmentRequestDto
    {
        [Required]
        public Guid PatientId { get; set; }

        [Range(1, int.MaxValue)]
        public int ClinicId { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }
    }
}
