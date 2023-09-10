using System.ComponentModel.DataAnnotations;

namespace Domain.DTO.Content;

public class AIContentCreationRequest
{
    [Required]
    public string TopicIndex { get; set; }
}
