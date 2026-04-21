using System.Collections.Generic;

namespace Hayat.BLL.DTOs.Receptionist
{
    public class ClinicWithSchedulesDto
    {
        public int ClinicId { get; set; }
        public string ClinicName { get; set; } = string.Empty;
        public List<ClinicScheduleDto> Schedules { get; set; } = new List<ClinicScheduleDto>();
    }
}
