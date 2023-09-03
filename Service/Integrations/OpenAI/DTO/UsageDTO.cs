namespace Service.Integrations.OpenAI.DTO;

public class UsageDTO
{
    public int PromptTokens { get; set; }
    public int CompletionTokens { get; set; }
    public int TotalTokens { get; set; }
}