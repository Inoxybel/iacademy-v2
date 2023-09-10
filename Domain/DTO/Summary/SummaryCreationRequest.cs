using System.ComponentModel.DataAnnotations;

namespace Domain.DTO.Summary;

public class SummaryCreationRequest
{
    [Required]
    public string Theme { get; set; }
    [Required]
    public string Category { get; set; }
    [Required]
    public string Subcategory { get; set; }
    [Required]
    public string ConfigurationId { get; set; }
    [Required]
    public string OwnerId { get; set; }
}
