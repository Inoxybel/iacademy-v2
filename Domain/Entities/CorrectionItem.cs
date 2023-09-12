namespace Domain.Entities;

public class CorrectionItem
{
    public int Identification { get; set; }
    public string Question { get; set; }
    public List<string> Complementation { get; set; }
    public string Answer { get; set; }
    public bool IsCorrect { get; set; }
    public string Feedback { get; set; }
}
