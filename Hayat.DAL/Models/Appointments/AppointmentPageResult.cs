using Hayat.DAL.Entities;

namespace Hayat.DAL.Models.Appointments
{
    public class AppointmentPageResult
    {
        public IReadOnlyList<Appointment> Items { get; set; } = Array.Empty<Appointment>();
        public bool HasMore { get; set; }
    }
}
