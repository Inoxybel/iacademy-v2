using Domain.Entities.Summary;
using SummaryEntity = Domain.Entities.Summary.Summary;

namespace Domain.DTO.Summary;

public class SummaryRequest
{
    public string OriginId { get; set; }
    public string OwnerId { get; set; }
    public string ConfigurationId { get; set; }
    public string ChatId { get; set; }
    public bool IsAvaliable { get; set; }
    public bool ShouldGeneratePendency { get; set; }
    public string Category { get; set; }
    public string Subcategory { get; set; }
    public string Theme { get; set; }
    public string Icon { get; set; }
    public List<Topic> Topics { get; set; }

    public static implicit operator SummaryRequest(SummaryEntity summary)
    {
        return new()
        {
            OriginId = summary.OriginId,
            OwnerId = summary.OwnerId,
            ConfigurationId = summary.ConfigurationId,
            ChatId = summary.ChatId,
            IsAvaliable = summary.IsAvaliable,
            ShouldGeneratePendency = summary.ShouldGeneratePendency,
            Category = summary.Category,
            Subcategory = summary.Subcategory,
            Theme = summary.Theme,
            Icon = summary.Icon,
            Topics = summary.Topics
        };
    }
}
