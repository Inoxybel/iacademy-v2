using System.ComponentModel.DataAnnotations;

namespace Domain.DTO.Exercise;

public class MakeExerciseRequest
{
    [Required]
    public string ContentId { get; set; }
    [Required]
    public string TopicIndex { get; set; }
    [Required]
    public string Title { get; set; }
    [Required]
    public string OwnerId { get; set; }
}
