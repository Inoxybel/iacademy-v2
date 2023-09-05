using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Subtopic
{
    [Required]
    public string Index { get; set; }
    [Required]
    public string Title { get; set; }
    public string ContentId { get; set; }
}
