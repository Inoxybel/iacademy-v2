namespace Service.Integrations.OpenAI.DTO;

public class OpenAIResponse
{
    public string Id { get; set; }
    public string Object { get; set; }
    public string Created { get; set; }
    public string Model { get; set; }
    public UsageDTO Usage { get; set; }
    public List<ChoicesDTO> Choices { get; set; }
}