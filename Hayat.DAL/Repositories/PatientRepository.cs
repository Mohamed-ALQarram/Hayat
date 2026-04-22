using Hayat.DAL.Data;
using Hayat.DAL.Entities;
using Hayat.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Hayat.DAL.Repositories
{
    public class PatientRepository : GenericRepository<Patient>, IPatientRepository
    {
        public PatientRepository(HayatDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<Patient>> SearchAsync(string searchTerm, int take = 20, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return Array.Empty<Patient>();
            }

            var normalizedTerm = searchTerm.Trim();
            var searchPattern = $"%{normalizedTerm}%";
            var boundedTake = Math.Clamp(take, 1, 100);

            return await Query()
                .Where(patient =>
                    EF.Functions.Like(patient.PatientId.ToString(), searchPattern) ||
                    EF.Functions.Like(patient.FullName, searchPattern) ||
                    EF.Functions.Like(patient.NationalId, searchPattern) ||
                    EF.Functions.Like(patient.Phone, searchPattern))
                .OrderBy(patient => patient.FullName)
                .Take(boundedTake)
                .ToListAsync(cancellationToken);
        }
    }
}
