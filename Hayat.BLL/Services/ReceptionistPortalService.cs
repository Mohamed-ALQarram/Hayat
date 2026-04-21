using Hayat.BLL.DTOs.Receptionist;
using Hayat.BLL.DTOs.Shared;
using Hayat.BLL.Exceptions;
using Hayat.BLL.Interfaces;
using Hayat.DAL.Entities;
using Hayat.DAL.Entities.Enums;
using Hayat.DAL.Interfaces;
using Hayat.DAL.Models.Appointments;

namespace Hayat.BLL.Services
{
    public class ReceptionistPortalService : IReceptionistPortalService
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IClinicRepository _clinicRepository;
        private readonly IGenericRepository<Patient> _patientReadRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<ClinicSchedule> _clinicScheduleRepository;

        public ReceptionistPortalService(
            IPatientRepository patientRepository,
            IAppointmentRepository appointmentRepository,
            IClinicRepository clinicRepository,
            IGenericRepository<Patient> patientReadRepository,
            IDoctorRepository doctorRepository,
            IUnitOfWork unitOfWork,
            IGenericRepository<ClinicSchedule> clinicScheduleRepository)
        {
            _patientRepository = patientRepository;
            _appointmentRepository = appointmentRepository;
            _clinicRepository = clinicRepository;
            _patientReadRepository = patientReadRepository;
            _doctorRepository = doctorRepository;
            _unitOfWork = unitOfWork;
            _clinicScheduleRepository = clinicScheduleRepository;
            _doctorRepository = doctorRepository;
        }

        public async Task<IReadOnlyList<PatientSearchResultDto>> SearchPatientsAsync(string searchTerm, CancellationToken cancellationToken = default)
        {
            var patients = await _patientRepository.SearchAsync(searchTerm, cancellationToken: cancellationToken);

            return patients
                .Select(patient => new PatientSearchResultDto
                {
                    PatientId = patient.PatientId,
                    FullName = patient.FullName,
                    NationalId = patient.NationalId,
                    Phone = patient.Phone,
                    Gender = patient.Gender.ToString(),
                    DateOfBirth = patient.DateOfBirth
                })
                .ToList();
        }


        public async Task<IReadOnlyList<DoctorWithClinicsResponseDto>> GetDoctorsWithClinicsAsync(string? searchTerm, CancellationToken cancellationToken = default)
        {
            var doctors = await _doctorRepository.GetDoctorsWithClinicsAsync(searchTerm, cancellationToken);
            
            return doctors.Select(d => new DoctorWithClinicsResponseDto
            {
                Id = d.DoctorId,
                FullName = d.FullName,
                Specialization = d.Specialty,
                WorkingClinics = d.ClinicSchedules.Select(cs => new ClinicWorkingHoursDto
                {
                    ClinicId = cs.ClinicId,
                    ClinicName = cs.Clinic.ClinicName,
                    WorkingDay = cs.DayOfWeek.ToString(),
                    WorkingHours = $"{cs.StartTime:hh\\:mm} - {cs.EndTime:hh\\:mm}"
                }).ToList()
            }).ToList();
        }
        public async Task<IReadOnlyList<string>> GetDoctorSpecializationsAsync(CancellationToken cancellationToken = default)
        {
            return await _doctorRepository.GetDoctorSpecializationsAsync(cancellationToken);
        }
        public async Task<IReadOnlyList<ClinicWithSchedulesDto>> GetClinicsWithSchedulesAsync(CancellationToken cancellationToken = default)
        {
            var clinics = await _clinicRepository.GetClinicsWithSchedulesAsync(cancellationToken);
            
            return clinics.Select(c => new ClinicWithSchedulesDto
            {
                ClinicId = c.ClinicId,
                ClinicName = c.ClinicName,
                Schedules = c.ClinicSchedules
                    .GroupBy(cs => cs.DayOfWeek)
                    .Select(g => new ClinicScheduleDto
                    {
                        DayOfWeek = g.Key,
                        StartTime = g.Min(cs => cs.StartTime),
                        EndTime = g.Max(cs => cs.EndTime)
                    })
                    .ToList()
            }).ToList();
        }

        public async Task<RegisterPatientResponseDto> RegisterPatientAsync(RegisterPatientRequestDto request, CancellationToken cancellationToken = default)
        {
            var existingPatient = await _patientRepository.FirstOrDefaultAsync(
                p => p.NationalId == request.NationalId || p.Phone == request.Phone,
                cancellationToken);

            if (existingPatient != null)
            {
                throw new BusinessRuleException("A patient with the same National ID or Phone already exists.");
            }

            if (!Enum.TryParse<Gender>(request.Gender, true, out var parsedGender))
            {
                throw new BusinessRuleException("Invalid gender provided.");
            }

            var newPatient = new Patient
            {
                FullName = request.FullName,
                NationalId = request.NationalId,
                Gender = parsedGender,
                DateOfBirth = request.DateOfBirth,
                Phone = request.Phone,
                Address = request.Address
            };

            await _patientRepository.AddAsync(newPatient, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new RegisterPatientResponseDto
            {
                PatientId = newPatient.PatientId,
                FullName = newPatient.FullName,
                NationalId = newPatient.NationalId,
                Phone = newPatient.Phone
            };
        }

        public async Task<AppointmentSummaryDto> BookAppointmentAsync(Guid branchId, BookAppointmentRequestDto request, CancellationToken cancellationToken = default)
        {
            if (request.AppointmentDate < DateTime.Today)
            {
                throw new BusinessRuleException("Appointments cannot be booked in the past.");
            }

            var clinic = await _clinicRepository.FirstOrDefaultAsync(
                existingClinic => existingClinic.ClinicId == request.ClinicId && existingClinic.BranchId == branchId,
                cancellationToken);

            if (clinic is null)
            {
                throw new EntityNotFoundException("Clinic was not found for the current branch.");
            }

            var patient = await _patientReadRepository.GetByIdAsync(request.PatientId, cancellationToken);
            if (patient is null)
            {
                throw new EntityNotFoundException("Patient was not found.");
            }

            var alreadyBooked = await _appointmentRepository.AnyAsync(
                appointment =>
                    appointment.ClinicId == request.ClinicId &&
                    appointment.PatientId == request.PatientId &&
                    appointment.AppointmentDate == request.AppointmentDate &&
                    appointment.Status != AppointmentStatus.Cancelled,
                cancellationToken);

            if (alreadyBooked)
            {
                throw new BusinessRuleException("An appointment already exists for the same patient, clinic, and time.");
            }

            var appointment = new Appointment
            {
                AppointmentDate = request.AppointmentDate,
                CreatedAt = DateTime.Now,
                ClinicId = clinic.ClinicId,
                PatientId = patient.PatientId,
                Status = AppointmentStatus.Scheduled
            };

            await _appointmentRepository.AddAsync(appointment, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var clinicDay = MapClinicDay(request.AppointmentDate.DayOfWeek);
            var schedule = await _clinicScheduleRepository.FirstOrDefaultAsync(
                s => s.ClinicId == clinic.ClinicId && s.DayOfWeek == clinicDay,
                cancellationToken);

            Guid? doctorId = schedule?.DoctorId;
            string doctorName = string.Empty;
            if (doctorId.HasValue)
            {
                var doctor = await _doctorRepository.GetByIdAsync(doctorId.Value, cancellationToken);
                doctorName = doctor?.FullName ?? string.Empty;
            }

            return MapAppointment(appointment, clinic.ClinicName, patient.FullName, doctorId, doctorName);
        }

        public async Task<TodayAppointmentsResponseDto> GetTodayAppointmentsAsync(Guid branchId, TodayAppointmentsQueryDto request, CancellationToken cancellationToken = default)
        {
            var effectiveStatus = ParseStatus(request.Status);
            var effectiveCursor = ParseCursor(request.Cursor);
            var effectiveSearch = request.AppointmentId.HasValue
                ? null
                : string.IsNullOrWhiteSpace(request.Search) ? null : request.Search.Trim();

            var query = new ReceptionTodayAppointmentsQuery
            {
                BranchId = branchId,
                Date = DateOnly.FromDateTime(DateTime.Today),
                Search = effectiveSearch,
                AppointmentId = request.AppointmentId,
                Status = effectiveStatus,
                ClinicId = request.ClinicId,
                Limit = request.Limit,
                Cursor = request.AppointmentId.HasValue ? null : effectiveCursor
            };

            var result = await _appointmentRepository.GetReceptionTodayAppointmentsPageAsync(query, cancellationToken);
            var items = result.Items.Select(MapAppointment).ToList();
            var lastItem = items.LastOrDefault();

            return new TodayAppointmentsResponseDto
            {
                Filters = new TodayAppointmentsFiltersDto
                {
                    Search = effectiveSearch,
                    AppointmentId = request.AppointmentId,
                    Status = effectiveStatus?.ToString(),
                    ClinicId = request.ClinicId
                },
                Items = items,
                PageInfo = new CursorPageInfoDto
                {
                    Limit = request.Limit,
                    HasMore = result.HasMore,
                    NextCursor = result.HasMore && lastItem is not null
                        ? CreateCursor(lastItem.AppointmentDate, lastItem.AppointmentId)
                        : null
                }
            };
        }

        public async Task<AppointmentSummaryDto> UpdateAppointmentStatusAsync(Guid branchId, int appointmentId, UpdateReceptionAppointmentStatusRequestDto request, CancellationToken cancellationToken = default)
        {
            var requestedStatus = ParseRequiredStatus(request.Status);
            if (requestedStatus != AppointmentStatus.Waiting)
            {
                throw new BusinessRuleException("Receptionist can only set appointment status to Waiting.");
            }

            var appointment = await _appointmentRepository.GetBranchScopedAppointmentAsync(branchId, appointmentId, cancellationToken);
            if (appointment is null)
            {
                throw new EntityNotFoundException("Appointment was not found for the current branch.");
            }

            if (appointment.Status == AppointmentStatus.Waiting)
            {
                return MapAppointment(appointment);
            }

            if (appointment.Status != AppointmentStatus.Scheduled)
            {
                throw new BusinessRuleException($"Receptionist cannot move appointment status from {appointment.Status} to Waiting.");
            }

            appointment.Status = AppointmentStatus.Waiting;
            _appointmentRepository.Update(appointment);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return MapAppointment(appointment);
        }

        private static AppointmentSummaryDto MapAppointment(Appointment appointment, string clinicName, string patientName, Guid? doctorId, string doctorName)
        {
            return new AppointmentSummaryDto
            {
                AppointmentId = appointment.AppointmentId,
                AppointmentDate = appointment.AppointmentDate,
                CreatedAt = appointment.CreatedAt,
                Status = appointment.Status.ToString(),
                ClinicId = appointment.ClinicId,
                ClinicName = clinicName,
                PatientId = appointment.PatientId,
                PatientName = patientName,
                DoctorId = doctorId,
                DoctorName = doctorName
            };
        }

        private static AppointmentSummaryDto MapAppointment(Appointment appointment)
        {
            var appointmentDay = MapClinicDay(appointment.AppointmentDate.DayOfWeek);
            var schedule = appointment.Clinic?.ClinicSchedules?.FirstOrDefault(existingSchedule => existingSchedule.DayOfWeek == appointmentDay);

            return MapAppointment(
                appointment,
                appointment.Clinic?.ClinicName ?? string.Empty,
                appointment.Patient?.FullName ?? string.Empty,
                schedule?.DoctorId,
                schedule?.Doctor?.FullName ?? string.Empty);
        }

        private static AppointmentStatus? ParseStatus(string? status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                return null;
            }

            if (!Enum.TryParse<AppointmentStatus>(status.Trim(), true, out var parsedStatus))
            {
                throw new BusinessRuleException("Invalid appointment status filter.");
            }

            return parsedStatus;
        }

        private static AppointmentStatus ParseRequiredStatus(string status)
        {
            if (!Enum.TryParse<AppointmentStatus>(status.Trim(), true, out var parsedStatus))
            {
                throw new BusinessRuleException("Invalid appointment status value.");
            }

            return parsedStatus;
        }

        private static AppointmentCursor? ParseCursor(string? cursor)
        {
            if (string.IsNullOrWhiteSpace(cursor))
            {
                return null;
            }

            var parts = cursor.Split('_', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2 ||
                !long.TryParse(parts[0], out var ticks) ||
                !int.TryParse(parts[1], out var appointmentId) ||
                appointmentId <= 0)
            {
                throw new BusinessRuleException("Invalid cursor.");
            }

            return new AppointmentCursor
            {
                AppointmentDate = new DateTime(ticks),
                AppointmentId = appointmentId
            };
        }

        private static string CreateCursor(DateTime appointmentDate, int appointmentId)
        {
            return $"{appointmentDate.Ticks}_{appointmentId}";
        }

        private static ClinicDayOfWeek MapClinicDay(DayOfWeek dayOfWeek)
        {
            return dayOfWeek switch
            {
                DayOfWeek.Saturday => ClinicDayOfWeek.Saturday,
                DayOfWeek.Sunday => ClinicDayOfWeek.Sunday,
                DayOfWeek.Monday => ClinicDayOfWeek.Monday,
                DayOfWeek.Tuesday => ClinicDayOfWeek.Tuesday,
                DayOfWeek.Wednesday => ClinicDayOfWeek.Wednesday,
                DayOfWeek.Thursday => ClinicDayOfWeek.Thursday,
                DayOfWeek.Friday => ClinicDayOfWeek.Friday,
                _ => throw new ArgumentOutOfRangeException(nameof(dayOfWeek), dayOfWeek, "Unsupported day of week.")
            };
        }
    }

}
