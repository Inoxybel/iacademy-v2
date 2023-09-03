namespace Service.Integrations.OpenAI.DTO;

public class ChoicesDTO
{
    public MessageDTO Message { get; set; }
    public string FinishReason { get; set; }
    public int Index { get; set; }
}