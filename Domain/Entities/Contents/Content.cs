namespace Domain.Entities.Contents;

public class Content
{
    public string Id { get; set; }
    public string ChatId { get; set; }
    public string OwnerId { get; set; }
    public string SummaryId { get; set; }
    public string ConfigurationId { get; set; }
    public string ExerciseId { get; set; }
    public string Theme { get; set; }
    public string SubtopicIndex { get; set; }
    public string Title { get; set; }
    public Body Body { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
}
