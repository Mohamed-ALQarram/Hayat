using Hayat.DAL.Entities;
using Hayat.DAL.Models.Appointments;

namespace Hayat.DAL.Interfaces
{
    public interface IAppointmentRepository : IGenericRepository<Appointment>
    {
        Task<IReadOnlyList<Appointment>> GetDoctorQueueAsync(Guid doctorId, DateOnly date, CancellationToken cancellationToken = default);
        Task<AppointmentPageResult> GetReceptionTodayAppointmentsPageAsync(ReceptionTodayAppointmentsQuery query, CancellationToken cancellationToken = default);
        Task<Appointment?> GetBranchScopedAppointmentAsync(Guid branchId, int appointmentId, CancellationToken cancellationToken = default);
    }
}
