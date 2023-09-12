using Domain.Entities;

namespace Domain.DTO.Content;

public class ContentRequest
{
    public string OwnerId { get; set; }
    public string SummaryId { get; set; }
    public string ConfigurationId { get; set; }
    public string ExerciseId { get; set; }
    public string Theme { get; set; }
    public string SubtopicIndex { get; set; }
    public string Title { get; set; }
    public List<Body> Body { get; set; }
}
