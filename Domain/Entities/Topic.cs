namespace Domain.Entities;

public class Topic
{
    public string Index { get; set; }
    public string Title { get; set; }
    public string Description { get; set; } 
    public List<Subtopic> Subtopics { get; set; }
}

