using System;
using System.Collections.Generic;

namespace Hayat.BLL.DTOs.Receptionist
{
    public class DoctorWithClinicsResponseDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public List<ClinicWorkingHoursDto> WorkingClinics { get; set; } = new List<ClinicWorkingHoursDto>();
    }
}
