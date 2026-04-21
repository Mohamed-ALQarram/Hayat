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


        [HttpPost("patients")]
        public async Task<ActionResult<RegisterPatientResponseDto>> RegisterPatient([FromBody] RegisterPatientRequestDto request, CancellationToken cancellationToken)
        {
            var patient = await _receptionistPortalService.RegisterPatientAsync(request, cancellationToken);
            return Ok(patient);
        }

        [HttpPost("appointments/book")]
        public async Task<ActionResult<AppointmentSummaryDto>> Book([FromBody] BookAppointmentRequestDto request, CancellationToken cancellationToken)
        {
            if (!User.TryGetBranchId(out var branchId))
            {
                return Forbid();
            }

            var appointment = await _receptionistPortalService.BookAppointmentAsync(branchId, request, cancellationToken);
            return Ok(appointment);
        }

        [HttpGet("appointments/today")]
        public async Task<ActionResult<TodayAppointmentsResponseDto>> GetTodayAppointments([FromQuery] TodayAppointmentsQueryDto request, CancellationToken cancellationToken)
        {
            if (!User.TryGetBranchId(out var branchId))
            {
                return Forbid();
            }

            var appointments = await _receptionistPortalService.GetTodayAppointmentsAsync(branchId, request, cancellationToken);
            return Ok(appointments);
        }

        [HttpPatch("appointments/{appointmentId:int}/status")]
        public async Task<ActionResult<AppointmentSummaryDto>> UpdateAppointmentStatus(int appointmentId, [FromBody] UpdateReceptionAppointmentStatusRequestDto request, CancellationToken cancellationToken)
        {
            if (!User.TryGetBranchId(out var branchId))
            {
                return Forbid();
            }

            var appointment = await _receptionistPortalService.UpdateAppointmentStatusAsync(branchId, appointmentId, request, cancellationToken);
            return Ok(appointment);
        }
    }
}
