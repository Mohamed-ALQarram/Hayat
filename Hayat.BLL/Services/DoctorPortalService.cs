using Hayat.BLL.DTOs.Doctor;
using Hayat.BLL.Interfaces;
using Hayat.DAL.Entities.Enums;
using Hayat.DAL.Interfaces;

namespace Hayat.BLL.Services
{
    public class DoctorPortalService : IDoctorPortalService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IVisitsHistoryRepository _visitsHistoryRepository;

        public DoctorPortalService(
            IAppointmentRepository appointmentRepository,
            IVisitsHistoryRepository visitsHistoryRepository)
        {
            _appointmentRepository = appointmentRepository;
            _visitsHistoryRepository = visitsHistoryRepository;
        }

        public async Task<IReadOnlyList<DoctorQueueItemDto>> GetQueueAsync(Guid doctorId, DateOnly date, CancellationToken cancellationToken = default)
        {
            var appointments = await _appointmentRepository.GetDoctorQueueAsync(doctorId, date, cancellationToken);

            return appointments
                .Where(appointment =>
                    appointment.Status != AppointmentStatus.Cancelled &&
                    appointment.Status != AppointmentStatus.Completed)
                .Select(appointment => new DoctorQueueItemDto
                {
                    AppointmentId = appointment.AppointmentId,
                    AppointmentDate = appointment.AppointmentDate,
                    Status = appointment.Status.ToString(),
                    PatientId = appointment.PatientId,
                    PatientName = appointment.Patient.FullName,
                    Gender = appointment.Patient.Gender,
                    Age = DateTime.Now.Year - appointment.Patient.DateOfBirth.Year,
                    Phone = appointment.Patient.Phone,
                    ClinicId = appointment.ClinicId,
                    ClinicName = appointment.Clinic.ClinicName
                })
                .ToList();
        }

        public async Task<IReadOnlyList<MedicalHistoryTimelineItemDto>> GetMedicalHistoryTimelineAsync(Guid patientId, CancellationToken cancellationToken = default)
        {
            var timeline = await _visitsHistoryRepository.GetPatientTimelineAsync(patientId, cancellationToken);

            return timeline
                .Select(history => new MedicalHistoryTimelineItemDto
                {
                    VisitId = history.Id,
                    CreatedAt = history.CreatedAt,
                    DoctorId = history.DoctorId,
                    DoctorName = history.Doctor.FullName,
                    Complaint = history.PatientComplaint,
                    Diagnosis = history.Diagnosis,
                    Notes = history.Notes,
                    Prescriptions = history.Prescriptions?
                    .Select(p=> 
                    new PrescriptionDto(p.DrugName, p.Dosage, p.Frequency,p.Duration, p.Instructions))
                    .ToList(),
                })
                .ToList();
        }
    }
}
