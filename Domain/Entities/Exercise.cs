using CrossCutting.Enums;

namespace Domain.Entities;

public class Exercise
{
    public string Id { get; set; }
    public string OwnerId { get; set; }
    public string CorrectionId { get; set; }
    public string ConfigurationId { get; set; }
    public ExerciseStatus Status { get; set; }
    public ExerciseType Type { get; set; }
    public DateTime SendedAt { get; set; }
    public string TopicIndex { get; set; }
    public string Title { get; set; }
    public List<Activity> Exercises { get; set; }
}
