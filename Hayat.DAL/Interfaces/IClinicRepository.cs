using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Hayat.DAL.Entities;

namespace Hayat.DAL.Interfaces
{
    public interface IClinicRepository : IGenericRepository<Clinic>
    {
        Task<IReadOnlyList<Clinic>> GetClinicsWithSchedulesAsync(CancellationToken cancellationToken = default);
    }
}
