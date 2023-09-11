using Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Domain.DTO.Content;

public class ContentRequest
{
    [Required]
    public string OwnerId { get; set; }
    [Required]
    public string SummaryId { get; set; }
    [Required]
    public string ConfigurationId { get; set; }
    public string ExerciseId { get; set; }
    [Required]
    public string Theme { get; set; }
    [Required]
    public string SubtopicIndex { get; set; }
    [Required]
    public string Title { get; set; }
    [Required]
    public List<Body> Body { get; set; }
}
