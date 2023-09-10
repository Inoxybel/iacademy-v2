using Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Domain.DTO.Correction;

public class CorrectionUpdateRequest
{
    [Required]
    public string OwnerId { get; set; }
    [Required]
    public string ExerciseId { get; set; }
    [Required]
    public List<CorrectionItem> Corrections { get; set; }
}
