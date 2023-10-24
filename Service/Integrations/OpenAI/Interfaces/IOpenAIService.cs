using Domain.Entities.Chat;
using Domain.Entities.Configuration;
using Service.Integrations.OpenAI.DTO;

namespace Service.Integrations.OpenAI.Interfaces;

public interface IOpenAIService
{
    Task<OpenAIResponse> DoRequest(InputProperties configurations, string userInput, string textGenre = "Informative");
    Task<OpenAIResponse> DoRequest(ChatCompletion chatCompletion, InputProperties configurations, string userInput, string textGenre = "Informative");
}