using Hayat.API.Infrastructure;
using Hayat.BLL.Constants;
using Hayat.BLL.DTOs.Receptionist;
using Hayat.BLL.DTOs.Shared;
using Hayat.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hayat.API.Controllers
{
    [ApiController]
    [Authorize(Roles = ApplicationRoles.Receptionist)]
    [Route("api/reception")]
    public class ReceptionistController : ControllerBase
    {
        private readonly IReceptionistPortalService _receptionistPortalService;

        public ReceptionistController(IReceptionistPortalService receptionistPortalService)
        {
            _receptionistPortalService = receptionistPortalService;
        }

        [HttpGet("patients/search")]
        public async Task<ActionResult<IReadOnlyList<PatientSearchResultDto>>> SearchPatients([FromQuery] string term, CancellationToken cancellationToken)
        {
            var results = await _receptionistPortalService.SearchPatientsAsync(term, cancellationToken);
            return Ok(results);
        }

        [HttpGet("doctors")]
        public async Task<ActionResult<IReadOnlyList<DoctorWithClinicsResponseDto>>> GetDoctors([FromQuery] string? search, CancellationToken cancellationToken)
        {
            var doctors = await _receptionistPortalService.GetDoctorsWithClinicsAsync(search, cancellationToken);
            return Ok(doctors);
        }

        [HttpGet("doctors/specializations")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetDoctorSpecializations(CancellationToken cancellationToken)
        {
            var specs = await _receptionistPortalService.GetDoctorSpecializationsAsync(cancellationToken);
            return Ok(specs);
        }

        [HttpGet("clinics-with-schedules")]
        public async Task<ActionResult<IReadOnlyList<ClinicWithSchedulesDto>>> GetClinicsWithSchedules(CancellationToken cancellationToken)
        {
            var clinics = await _receptionistPortalService.GetClinicsWithSchedulesAsync(cancellationToken);
            return Ok(clinics);
        }

        [HttpPost("appointments/quick-book")]
        public async Task<ActionResult<AppointmentSummaryDto>> QuickBook([FromBody] QuickBookAppointmentRequestDto request, CancellationToken cancellationToken)
        {
            if (!User.TryGetBranchId(out var branchId))
            {
                return Forbid();
            }

            var appointment = await _receptionistPortalService.QuickBookAsync(branchId, request, cancellationToken);
            return Ok(appointment);
        }

        [HttpGet("appointments/today")]
        public async Task<ActionResult<IReadOnlyList<AppointmentSummaryDto>>> GetTodayAppointments([FromQuery] DateOnly? date, CancellationToken cancellationToken)
        {
            if (!User.TryGetBranchId(out var branchId))
            {
                return Forbid();
            }

            var targetDate = date ?? DateOnly.FromDateTime(DateTime.Today);
            var appointments = await _receptionistPortalService.GetAppointmentsForDateAsync(branchId, targetDate, cancellationToken);
            return Ok(appointments);
        }
    }
}
