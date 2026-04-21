namespace Hayat.DAL.Entities
{
    public class VisitsHistory
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string PatientComplaint { get; set; } = string.Empty;
        public string Diagnosis { get; set; } = string.Empty;
        public string? Notes { get; set; } 

        public ICollection<Prescription>? Prescriptions { get; set; } = new List<Prescription>();
        public Guid PatientId { get; set; }
        public virtual Patient Patient { get; set; } = null!;

        public Guid DoctorId { get; set; }
        public virtual Doctor Doctor { get; set; } = null!;
    }
}