using Hayat.BLL.DTOs.Receptionist;
using Hayat.BLL.DTOs.Shared;

namespace Hayat.BLL.Interfaces
{
    public interface IReceptionistPortalService
    {
        Task<IReadOnlyList<PatientSearchResultDto>> SearchPatientsAsync(string searchTerm, CancellationToken cancellationToken = default);
        Task<AppointmentSummaryDto> BookAppointmentAsync(Guid branchId, BookAppointmentRequestDto request, CancellationToken cancellationToken = default);
        Task<TodayAppointmentsResponseDto> GetTodayAppointmentsAsync(Guid branchId, TodayAppointmentsQueryDto request, CancellationToken cancellationToken = default);
        Task<AppointmentSummaryDto> UpdateAppointmentStatusAsync(Guid branchId, int appointmentId, UpdateReceptionAppointmentStatusRequestDto request, CancellationToken cancellationToken = default);
        Task<RegisterPatientResponseDto> RegisterPatientAsync(RegisterPatientRequestDto request, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<DoctorWithClinicsResponseDto>> GetDoctorsWithClinicsAsync(string? searchTerm, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<string>> GetDoctorSpecializationsAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ClinicWithSchedulesDto>> GetClinicsWithSchedulesAsync(CancellationToken cancellationToken = default);
    }
}
