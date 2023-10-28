namespace Domain.DTO.Summary;

public class TopicResumeDTO
{
    public string Index { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public List<SubtopicResumeDTO> Subtopics { get; set; }
}
