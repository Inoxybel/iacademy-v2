using System.ComponentModel.DataAnnotations;

namespace Domain.DTO.Summary;

public class SummarySubtopicDTO
{
    [Required]
    public string Index { get; set; }
    [Required]
    public string Title { get; set; }
    [Required]
    public string Content { get; set; }
}
