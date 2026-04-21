using Hayat.DAL.Data;
using Hayat.DAL.Entities;
using Hayat.DAL.Entities.Enums;
using Hayat.DAL.Interfaces;
using Hayat.DAL.Models.Appointments;
using Microsoft.EntityFrameworkCore;

namespace Hayat.DAL.Repositories
{
    public class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
    {
        public AppointmentRepository(HayatDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<Appointment>> GetDoctorQueueAsync(Guid doctorId, DateOnly date, CancellationToken cancellationToken = default)
        {
            var (start, end) = CreateDateRange(date);
            var clinicDay = MapClinicDay(date.DayOfWeek);

            var clinicIds = Context.ClinicSchedules
                .AsNoTracking()
                .Where(schedule => schedule.DoctorId == doctorId && schedule.DayOfWeek == clinicDay)
                .Select(schedule => schedule.ClinicId);

            return await Context.Appointments
                .AsNoTracking()
                .Include(appointment => appointment.Patient)
                .Include(appointment => appointment.Clinic)
                .Where(appointment =>
                    appointment.AppointmentDate >= start &&
                    appointment.AppointmentDate < end &&
                    clinicIds.Contains(appointment.ClinicId))
                .OrderBy(appointment => appointment.AppointmentDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<AppointmentPageResult> GetReceptionTodayAppointmentsPageAsync(ReceptionTodayAppointmentsQuery query, CancellationToken cancellationToken = default)
        {
            var (start, end) = CreateDateRange(query.Date);

            var appointmentsQuery = CreateAppointmentReadQuery(asNoTracking: true)
                .Where(appointment =>
                    appointment.AppointmentDate >= start &&
                    appointment.AppointmentDate < end &&
                    appointment.Clinic.BranchId == query.BranchId);

            if (query.AppointmentId.HasValue)
            {
                appointmentsQuery = appointmentsQuery.Where(appointment => appointment.AppointmentId == query.AppointmentId.Value);
            }
            else if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var searchPattern = $"%{query.Search.Trim()}%";
                appointmentsQuery = appointmentsQuery.Where(appointment => EF.Functions.Like(appointment.Patient.FullName, searchPattern));
            }

            if (query.Status.HasValue)
            {
                appointmentsQuery = appointmentsQuery.Where(appointment => appointment.Status == query.Status.Value);
            }

            if (query.ClinicId.HasValue)
            {
                appointmentsQuery = appointmentsQuery.Where(appointment => appointment.ClinicId == query.ClinicId.Value);
            }

            if (query.AppointmentId is null && query.Cursor is not null)
            {
                appointmentsQuery = appointmentsQuery.Where(appointment =>
                    appointment.AppointmentDate > query.Cursor.AppointmentDate ||
                    (appointment.AppointmentDate == query.Cursor.AppointmentDate && appointment.AppointmentId > query.Cursor.AppointmentId));
            }

            var items = await appointmentsQuery
                .OrderBy(appointment => appointment.AppointmentDate)
                .ThenBy(appointment => appointment.AppointmentId)
                .Take(query.Limit + 1)
                .ToListAsync(cancellationToken);

            var hasMore = items.Count > query.Limit;
            if (hasMore)
            {
                items = items.Take(query.Limit).ToList();
            }

            return new AppointmentPageResult
            {
                Items = items,
                HasMore = hasMore
            };
        }

        public async Task<Appointment?> GetBranchScopedAppointmentAsync(Guid branchId, int appointmentId, CancellationToken cancellationToken = default)
        {
            return await CreateAppointmentReadQuery(asNoTracking: false)
                .Where(appointment => appointment.AppointmentId == appointmentId && appointment.Clinic.BranchId == branchId)
                .SingleOrDefaultAsync(cancellationToken);
        }

        private IQueryable<Appointment> CreateAppointmentReadQuery(bool asNoTracking)
        {
            IQueryable<Appointment> query = Context.Appointments
                .Include(appointment => appointment.Patient)
                .Include(appointment => appointment.Clinic)
                    .ThenInclude(clinic => clinic.ClinicSchedules)
                        .ThenInclude(schedule => schedule.Doctor)
                .AsSplitQuery();

            return asNoTracking ? query.AsNoTracking() : query;
        }

        private static (DateTime Start, DateTime End) CreateDateRange(DateOnly date)
        {
            var start = date.ToDateTime(TimeOnly.MinValue);
            return (start, start.AddDays(1));
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
