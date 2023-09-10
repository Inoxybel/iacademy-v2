using System.ComponentModel.DataAnnotations;

namespace Domain.DTO.Correction;

public class CreateCorrectionRequest
{
    [Required]
    public List<ActivityToCorrectDTO> Exercises { get; set; }
}
