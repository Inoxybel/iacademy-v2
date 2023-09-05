using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Topic
{
    [Required]
    public string Index { get; set; }
    [Required]
    public string Title { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    public List<Subtopic> Subtopics { get; set; }
}

