using Hayat.DAL.Entities.Enums;

namespace Hayat.DAL.Models.Appointments
{
    public class ReceptionTodayAppointmentsQuery
    {
        public Guid BranchId { get; set; }
        public DateOnly Date { get; set; }
        public string? Search { get; set; }
        public int? AppointmentId { get; set; }
        public AppointmentStatus? Status { get; set; }
        public int? ClinicId { get; set; }
        public int Limit { get; set; }
        public AppointmentCursor? Cursor { get; set; }
    }
}
