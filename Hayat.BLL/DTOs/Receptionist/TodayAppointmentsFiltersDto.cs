namespace Hayat.BLL.DTOs.Receptionist
{
    public class TodayAppointmentsFiltersDto
    {
        public string? Search { get; set; }
        public int? AppointmentId { get; set; }
        public string? Status { get; set; }
        public int? ClinicId { get; set; }
    }
}
