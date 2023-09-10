using System.ComponentModel.DataAnnotations;

namespace Domain.DTO.Summary;

public class SummaryContentsDTO
{
    [Required]
    public List<SummarySubtopicDTO> Subtopics { get; set; }

    public bool IsValid() => Subtopics != null && Subtopics.Any();
}

