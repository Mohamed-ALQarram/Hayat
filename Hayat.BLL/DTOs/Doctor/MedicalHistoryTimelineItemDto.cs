namespace Hayat.BLL.DTOs.Doctor
{
    public class MedicalHistoryTimelineItemDto
    {
        public Guid VisitId { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string Complaint { get; set; } = string.Empty;
        public string Diagnosis { get; set; } = string.Empty;
        public string? Notes { get; set; } = string.Empty;
        public List<PrescriptionDto>? Prescriptions { get; set; }
    }
}
