using System.ComponentModel.DataAnnotations;

namespace Hayat.BLL.DTOs.Receptionist
{
    public class TodayAppointmentsQueryDto
    {
        [StringLength(150)]
        public string? Search { get; set; }

        [Range(1, int.MaxValue)]
        public int? AppointmentId { get; set; }

        [StringLength(50)]
        public string? Status { get; set; }

        [Range(1, int.MaxValue)]
        public int? ClinicId { get; set; }

        [Range(1, 100)]
        public int Limit { get; set; } = 20;

        [StringLength(100)]
        public string? Cursor { get; set; }
    }
}
