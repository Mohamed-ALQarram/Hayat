using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Hayat.DAL.Entities;

namespace Hayat.DAL.Interfaces
{
    public interface IDoctorRepository : IGenericRepository<Doctor>
    {
        Task<IReadOnlyList<Doctor>> GetDoctorsWithClinicsAsync(string? searchTerm, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<string>> GetDoctorSpecializationsAsync(CancellationToken cancellationToken = default);
    }
}
