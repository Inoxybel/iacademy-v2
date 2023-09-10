using System.ComponentModel.DataAnnotations;

namespace Domain.DTO.Correction;

public class ActivityToCorrectDTO
{
    [Required]
    public int Identification { get; set; }
    [Required]
    public string Answer { get; set; }
}
