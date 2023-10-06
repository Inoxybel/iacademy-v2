namespace Domain.DTO.Correction;

public class CorrectionItemDTO
{
    public string Question { get; set; }
    public List<string> Complementation { get; set; }
    public string Answer { get; set; }
    public bool IsCorrect { get; set; }
}

