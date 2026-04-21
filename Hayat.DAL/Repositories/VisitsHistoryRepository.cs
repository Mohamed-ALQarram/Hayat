using Hayat.DAL.Data;
using Hayat.DAL.Entities;
using Hayat.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Hayat.DAL.Repositories
{
    public class VisitsHistoryRepository : GenericRepository<VisitsHistory>, IVisitsHistoryRepository
    {
        public VisitsHistoryRepository(HayatDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<VisitsHistory>> GetPatientTimelineAsync(Guid patientId, CancellationToken cancellationToken = default)
        {
            return await Context.VisitsHistories
                .AsNoTracking()
                .Include(history => history.Doctor)
                .Include(h=>h.Prescriptions)
                .AsSplitQuery()
                .Where(history => history.PatientId == patientId)
                .OrderByDescending(history => history.CreatedAt)
                .ToListAsync(cancellationToken);
        }
    }
}
