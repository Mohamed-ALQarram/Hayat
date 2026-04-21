using System.ComponentModel.DataAnnotations;

namespace Hayat.BLL.DTOs.Receptionist
{
    public class RegisterPatientRequestDto
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public string NationalId { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^(Male|Female)$", ErrorMessage = "Gender must be either 'Male' or 'Female'.")]
        public string Gender { get; set; } = string.Empty;

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public string Phone { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;
    }
}
