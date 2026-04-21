namespace Hayat.BLL.DTOs.Doctor
{
    public record PrescriptionDto(string DrugName,
    string Dosage,
    string Frequency,
    string Duration,
    string? Instructions);
}
