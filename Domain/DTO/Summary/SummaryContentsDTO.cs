namespace Domain.DTO.Summary;

public class SummaryContentsDTO
{
    public List<SummarySubtopicDTO> Subtopics { get; set; }

    public bool IsValid() => Subtopics != null && Subtopics.Any();
}

