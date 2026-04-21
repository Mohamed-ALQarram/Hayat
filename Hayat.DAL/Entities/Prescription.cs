namespace Hayat.DAL.Entities
{
    public class Prescription
    {
        public Guid Id { get; set; }
        public string DrugName { get; set; } = null!;
        public string Dosage { get; set; }=null!;
        public string Frequency { get; set; } = null!;
        public string Duration { get; set; } = null!;
        public string? Instructions { get; set; }
        public Guid VisitHistoryId { get; set; }
        public VisitsHistory visitsHistory { get; set; }= null!;
    }
}