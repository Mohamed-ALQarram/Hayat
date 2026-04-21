using Hayat.DAL.Data.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Hayat.DAL.Entities;

namespace Hayat.DAL.Data
{
    public class HayatDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public HayatDbContext(DbContextOptions<HayatDbContext> options) : base(options) { }

        public DbSet<Branch> Branches { get; set; }
        public DbSet<Clinic> Clinics { get; set; }
        public DbSet<ClinicSchedule> ClinicSchedules { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<VisitsHistory> VisitsHistories { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply all IEntityTypeConfiguration<T> classes from this assembly
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            modelBuilder.Entity<IdentityRole<Guid>>(builder =>
            {
                builder.Property(role => role.Id).HasValueGenerator<UuidV7Generator>();
            });
        }
    }
}
