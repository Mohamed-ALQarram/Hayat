using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hayat.DAL.Data;
using Hayat.DAL.Entities;
using Hayat.DAL.Entities.Enums;
using Hayat.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Hayat.DAL.Repositories
{
    public class DoctorRepository : GenericRepository<Doctor>, IDoctorRepository
    {
        public DoctorRepository(HayatDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<Doctor>> GetDoctorsWithClinicsAsync(string? searchTerm, CancellationToken cancellationToken = default)
        {
            var query = Context.Doctors
                .Include(d => d.ClinicSchedules)
                    .ThenInclude(cs => cs.Clinic)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                bool validGuid =Guid.TryParse(searchTerm, out Guid Id);
                bool validDay = Enum.TryParse(searchTerm, out ClinicDayOfWeek dayOfWeek);
                if (validGuid)
                    query = query.Where(d => d.DoctorId == Id);
                else if (validDay)
                    query = query.Where(d => d.ClinicSchedules.Any(cs =>
                                    cs.DayOfWeek == dayOfWeek));
                else
                    query = query.Where(d =>
                                d.FullName.Contains(searchTerm) ||
                                d.Specialty.Contains(searchTerm) ||
                                d.ClinicSchedules.Any(cs =>
                                    cs.Clinic.ClinicName.Contains(searchTerm)
                                ));
            }

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<string>> GetDoctorSpecializationsAsync(CancellationToken cancellationToken = default)
        {
            return await Context.Doctors
                .Select(d => d.Specialty)
                .Distinct()
                .ToListAsync(cancellationToken);
        }
    }
}
