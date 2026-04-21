using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hayat.DAL.Data;
using Hayat.DAL.Entities;
using Hayat.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace Hayat.DAL.Repositories
{
    public class ClinicRepository : GenericRepository<Clinic>, IClinicRepository
    {
        public ClinicRepository(HayatDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<Clinic>> GetClinicsWithSchedulesAsync(CancellationToken cancellationToken = default)
        {
            return await Context.Clinics
                .Include(c => c.ClinicSchedules)
                .AsSplitQuery()
                .ToListAsync(cancellationToken);
        }
    }
}
