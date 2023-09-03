namespace Service.Integrations.OpenAI.DTO;

public class OpenAIRequest
{
    public string Model { get; set; }
    public List<MessageDTO> Messages { get; set; }
    public double Temperature { get; set; }
}