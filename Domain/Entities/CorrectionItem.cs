using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class CorrectionItem
{
    [Required]
    public int Identification { get; set; }
    [Required]
    public string Question { get; set; }
    public List<string> Complementation { get; set; }
    [Required]
    public string Answer { get; set; }
    [Required]
    public bool IsCorrect { get; set; }
    [Required]
    public string Feedback { get; set; }
}
