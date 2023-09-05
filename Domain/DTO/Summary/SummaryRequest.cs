using Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Domain.DTO.Summary;

public class SummaryRequest
{
    public string OriginId { get; set; }
    [Required]
    public string OwnerId { get; set; }
    [Required]
    public string ConfigurationId { get; set; }
    public bool IsAvaliable { get; set; }
    [Required]
    public string Category { get; set; }
    [Required]
    public string Subcategory { get; set; }
    [Required]
    public string Theme { get; set; }
    [Required]
    public List<Topic> Topics { get; set; }
}
