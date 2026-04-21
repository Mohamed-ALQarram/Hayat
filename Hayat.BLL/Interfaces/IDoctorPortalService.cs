using Hayat.BLL.DTOs.Doctor;

namespace Hayat.BLL.Interfaces
{
    public interface IDoctorPortalService
    {
        Task<IReadOnlyList<DoctorQueueItemDto>> GetQueueAsync(Guid doctorId, DateOnly date, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<MedicalHistoryTimelineItemDto>> GetMedicalHistoryTimelineAsync(Guid patientId, CancellationToken cancellationToken = default);

        Task<string> WriteVisitHistory(Guid patientId, Guid doctorId, string patientComplaint, string diagnosis, string? notes, List<PrescriptionDto>? prescriptions, CancellationToken cancellationToken);
        Task<UpdateAppointmentStatusResponseDto> UpdateAppointmentStatusAsync(int appointmentId, UpdateAppointmentStatusRequestDto request, CancellationToken cancellationToken = default);
    }
}
