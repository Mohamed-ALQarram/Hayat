using Hayat.DAL.Entities.Enums;

namespace Hayat.BLL.DTOs.Receptionist
{
    public class ClinicScheduleDto
    {
        public ClinicDayOfWeek DayOfWeek { get; set; } 
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
