namespace Hayat.BLL.DTOs.Receptionist
{
    public class RegisterPatientResponseDto
    {
        public Guid PatientId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string NationalId { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }
}
