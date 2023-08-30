namespace Domain.Entities;

public class Correction
{
    public string Id { get; set; }
    public string ExerciseId { get; set; }
    public string OwnerId { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public List<CorrectionItem> Corrections { get; set; }
}
