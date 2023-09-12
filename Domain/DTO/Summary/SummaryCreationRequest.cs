

namespace Domain.DTO.Summary;

public class SummaryCreationRequest
{
    public string Theme { get; set; }
    public string Category { get; set; }
    public string Subcategory { get; set; }
    public string ConfigurationId { get; set; }
    public string OwnerId { get; set; }
}
