using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Body
{
    [Required]
    public string Content { get; set; }
    [Required]
    public DateTime CreatedDate { get; set; }
    public DateTime DisabledDate { get; set; }
}
