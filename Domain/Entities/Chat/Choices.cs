namespace Domain.Entities.Chat;

public class Choices
{
    public Message Message { get; set; }
    public string FinishReason { get; set; }
    public int Index { get; set; }
}
