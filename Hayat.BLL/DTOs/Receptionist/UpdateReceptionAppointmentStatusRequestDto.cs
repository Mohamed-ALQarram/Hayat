using System.ComponentModel.DataAnnotations;

namespace Hayat.BLL.DTOs.Receptionist
{
    public class UpdateReceptionAppointmentStatusRequestDto
    {
        [Required]
        [MinLength(1)]
        public string Status { get; set; } = string.Empty;
    }
}
