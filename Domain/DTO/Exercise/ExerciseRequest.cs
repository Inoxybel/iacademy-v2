using CrossCutting.Enums;
using Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Domain.DTO.Exercise;

public class ExerciseRequest
{
    [Required]
    public string OwnerId { get; set; }
    [Required]
    public string CorrectionId { get; set; }
    [Required]
    public string ConfigurationId { get; set; }
    public ExerciseStatus Status { get; set; }
    [Required]
    public ExerciseType Type { get; set; }
    public DateTime SendedAt { get; set; }
    [Required]
    public string TopicIndex { get; set; }
    [Required]
    public string Title { get; set; }
    [Required]
    public List<Activity> Exercises { get; set; }
}
