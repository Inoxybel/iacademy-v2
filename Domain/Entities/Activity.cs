using CrossCutting.Enums;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Activity
{
    [Required]
    public int Identification { get; set; }
    [Required]
    public ActivityType Type { get; set; }
    [Required]
    public string Question { get; set; }
    public List<string> Complementation { get; set; }
    public string Answer { get; set; }
}
