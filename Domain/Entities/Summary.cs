namespace Domain.Entities;

public class Summary
{
    public string Id { get; set; }
    public string OwnerId { get; set; }
    public string ConfigurationId { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public bool IsAvaliable { get; set; }
    public string Category { get; set; }
    public string Subcategory { get; set; }
    public string Theme { get; set; }

    public List<Topic> Topics { get; set; }
}
