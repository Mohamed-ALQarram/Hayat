using Hayat.BLL.DTOs.Shared;

namespace Hayat.BLL.DTOs.Receptionist
{
    public class TodayAppointmentsResponseDto
    {
        public TodayAppointmentsFiltersDto Filters { get; set; } = new();
        public IReadOnlyList<AppointmentSummaryDto> Items { get; set; } = Array.Empty<AppointmentSummaryDto>();
        public CursorPageInfoDto PageInfo { get; set; } = new();
    }
}
