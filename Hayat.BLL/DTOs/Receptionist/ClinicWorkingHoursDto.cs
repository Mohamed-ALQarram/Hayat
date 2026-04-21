using System;

namespace Hayat.BLL.DTOs.Receptionist
{
    public class ClinicWorkingHoursDto
    {
        public int ClinicId { get; set; }
        public string ClinicName { get; set; } = string.Empty;
        public string WorkingDay { get; set; } = string.Empty;
        public string WorkingHours { get; set; } = string.Empty;
    }
}
