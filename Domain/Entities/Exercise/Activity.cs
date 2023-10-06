using CrossCutting.Enums;

namespace Domain.Entities.Exercise;

public class Activity
{
    public int Identification { get; set; }
    public ActivityType Type { get; set; }
    public string Question { get; set; }
    public List<string> Complementation { get; set; }
    public string Answer { get; set; }
}
