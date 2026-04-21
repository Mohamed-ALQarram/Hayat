using Hayat.BLL.DTOs.Doctor;
using Hayat.BLL.Interfaces;
using Hayat.DAL.Entities;
using Hayat.DAL.Entities.Enums;
using Hayat.DAL.Interfaces;
using static Azure.Core.HttpHeader;

namespace Hayat.BLL.Services
{
    public class DoctorPortalService : IDoctorPortalService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IVisitsHistoryRepository _visitsHistoryRepository;
        private readonly IGenericRepository<VisitsHistory> visitHistoryRepo;
        private readonly IUnitOfWork unitOfWork;

        public DoctorPortalService(
            IAppointmentRepository appointmentRepository,
            IVisitsHistoryRepository visitsHistoryRepository,
            IGenericRepository<VisitsHistory> VisitHistoryRepo,
            IUnitOfWork unitOfWork)
        {
            _appointmentRepository = appointmentRepository;
            _visitsHistoryRepository = visitsHistoryRepository;
            visitHistoryRepo = VisitHistoryRepo;
            this.unitOfWork = unitOfWork;
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

        public async Task<string> WriteVisitHistory(Guid patientId, Guid doctorId, string patientComplaint, string diagnosis, string? notes,
            List<PrescriptionDto>? prescriptions,
            CancellationToken cancellationToken)
        {
            List<Prescription> prescriptionList = null;
            if(prescriptions is not null)
            {
                prescriptionList = (prescriptions.Select(p=> 
                new Prescription { Dosage= p.Dosage, DrugName = p.DrugName, Duration = p.Duration, Frequency= p.Frequency, Instructions = p.Instructions}).ToList());
            }

            var history = new VisitsHistory
            {
                PatientId = patientId,
                DoctorId = doctorId,
                PatientComplaint = patientComplaint,
                Diagnosis = diagnosis,
                Notes = notes,
                Prescriptions = prescriptionList
            };
            await visitHistoryRepo.AddAsync(history);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return "History saved successfully";
        }

        public async Task<UpdateAppointmentStatusResponseDto> UpdateAppointmentStatusAsync(int appointmentId, UpdateAppointmentStatusRequestDto request, CancellationToken cancellationToken = default)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);

            if (appointment == null)
            {
                throw new Exceptions.EntityNotFoundException("Appointment not found.");
            }

            if (request.Status == AppointmentStatus.Scheduled)
            {
                throw new Exceptions.BusinessRuleException("Doctor cannot move appointment status back to Scheduled.");
            }

            appointment.Status = request.Status;

            _appointmentRepository.Update(appointment);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return new UpdateAppointmentStatusResponseDto
            {
                AppointmentId = appointment.AppointmentId,
                Status = appointment.Status,
                Updated = true
            };
        }
    }
}
