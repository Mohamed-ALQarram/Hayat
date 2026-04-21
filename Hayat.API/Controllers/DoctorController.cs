using Hayat.API.DTOs;
using Hayat.API.Infrastructure;
using Hayat.BLL.Constants;
using Hayat.BLL.DTOs.Doctor;
using Hayat.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hayat.API.Controllers
{
    [ApiController]
    [Authorize(Roles = ApplicationRoles.Doctor)]
    [Route("api/doctor")]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorPortalService _doctorPortalService;

        public DoctorController(IDoctorPortalService doctorPortalService)
        {
            _doctorPortalService = doctorPortalService;
        }

        [HttpGet("queue")]
        public async Task<ActionResult<IReadOnlyList<DoctorQueueItemDto>>> GetQueue([FromQuery] DateOnly? date, CancellationToken cancellationToken)
        {
            if (!User.TryGetDoctorId(out var doctorId))
            {
                return Forbid();
            }

            var targetDate = date ?? DateOnly.FromDateTime(DateTime.Today);
            var queue = await _doctorPortalService.GetQueueAsync(doctorId, targetDate, cancellationToken);
            return Ok(queue);
        }

        [HttpGet("patients/{patientId:guid}/medical-history")]
        public async Task<ActionResult<IReadOnlyList<MedicalHistoryTimelineItemDto>>> GetMedicalHistory(Guid patientId, CancellationToken cancellationToken)
        {
            var timeline = await _doctorPortalService.GetMedicalHistoryTimelineAsync(patientId, cancellationToken);
            return Ok(timeline);
        }

        [HttpPost("patients/{patientId:guid}/medical-history")]
        public async Task<ActionResult> WriteVisitHistory(Guid patientId, 
            WriteVisitHistoryRequest visitHistoryRequest,
            CancellationToken cancellationToken)
        {
            if (!User.TryGetDoctorId(out var doctorId))
            {
                return Forbid();
            }

            var result = await _doctorPortalService.WriteVisitHistory(patientId,
                doctorId,
                visitHistoryRequest.PatientComplaint,
                visitHistoryRequest.Diagnosis,
                visitHistoryRequest.Notes, visitHistoryRequest.prescriptions, cancellationToken);
            return Ok(result);
        }

        [HttpPatch("appointments/{appointmentId:int}/status")]
        public async Task<ActionResult<UpdateAppointmentStatusResponseDto>> UpdateStatus(int appointmentId, [FromBody] UpdateAppointmentStatusRequestDto request, CancellationToken cancellationToken)
        {
            var response = await _doctorPortalService.UpdateAppointmentStatusAsync(appointmentId, request, cancellationToken);
            return Ok(response);
        }
    }
}
