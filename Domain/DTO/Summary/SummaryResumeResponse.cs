namespace Domain.DTO.Summary;

public class SummaryResumeResponse
{
    public string Id { get; set; }
    public string Icon { get; set; }
    public bool IsAvaliable { get; set; }
    public string Category { get; set; }
    public bool ShouldGeneratePendency { get; set; }
    public string Subcategory { get; set; }
    public string Theme { get; set; }
    public List<TopicResumeDTO> Topics { get; set; }
}
