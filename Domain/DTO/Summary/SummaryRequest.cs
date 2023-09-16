using Domain.Entities;

namespace Domain.DTO.Summary;

public class SummaryRequest
{
    public string OriginId { get; set; }
    public string OwnerId { get; set; }
    public string ConfigurationId { get; set; }
    public string ChatId { get; set; }
    public bool IsAvaliable { get; set; }
    public string Category { get; set; }
    public string Subcategory { get; set; }
    public string Theme { get; set; }
    public string Icon { get; set; }
    public List<Topic> Topics { get; set; }
}
