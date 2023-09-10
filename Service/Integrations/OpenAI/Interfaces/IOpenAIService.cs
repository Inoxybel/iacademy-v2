using Domain.Entities;
using Service.Integrations.OpenAI.DTO;

namespace Service.Integrations.OpenAI.Interfaces;

public interface IOpenAIService
{
    Task<OpenAIResponse> DoRequest(string objStringified);
    Task<OpenAIResponse> DoRequest(ChatCompletion chatCompletion, string prompt);
}