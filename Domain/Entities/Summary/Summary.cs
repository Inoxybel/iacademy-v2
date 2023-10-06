namespace Domain.Entities.Summary;

public class Summary
{
    public string Id { get; set; }
    public string OriginId { get; set; }
    public string OwnerId { get; set; }
    public string ConfigurationId { get; set; }
    public string ChatId { get; set; }
    public string Icon { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public bool IsAvaliable { get; set; }
    public string Category { get; set; }
    public bool ShouldGeneratePendency { get; set; }
    public string Subcategory { get; set; }
    public string Theme { get; set; }
    public List<Topic> Topics { get; set; }
}
