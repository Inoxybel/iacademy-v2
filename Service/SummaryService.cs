using Domain.DTO;
using Domain.DTO.Summary;
using Domain.Entities;
using Domain.Infra;
using Domain.Services;
using Newtonsoft.Json;
using Service.Integrations.OpenAI.DTO;
using Service.Integrations.OpenAI.Interfaces;

namespace Service;

public class SummaryService : ISummaryService
{
    private readonly ISummaryRepository _repository;
    private readonly IConfigurationService _configurationService;
    private readonly IOpenAIService _openAIService;
    private readonly IChatCompletionsService _chatCompletionsService;

    public SummaryService(
        ISummaryRepository repository,
        IConfigurationService configurationService,
        IOpenAIService openAIService,
        IChatCompletionsService chatCompletionsService)
    {
        _repository = repository;
        _configurationService = configurationService;
        _openAIService = openAIService;
        _chatCompletionsService = chatCompletionsService;
    }

    public async Task<ServiceResult<Summary>> Get(string id, CancellationToken cancellationToken = default)
    {
        var summary = await _repository.Get(id, cancellationToken);

        if (summary is null)
            return MakeErrorResult<Summary>("Summary not found.");

        return new()
        {
            Data = summary,
            Success = true
        };
    }

    public async Task<ServiceResult<List<Summary>>> GetAllByCategory(string category, bool isAvaliable = false, CancellationToken cancellationToken = default)
    {
        var summaries = await _repository.GetAllByCategory(category, isAvaliable, cancellationToken);

        if (summaries.Any())
        {
            return new()
            {
                Data = summaries,
                Success = true
            };
        }

        return MakeErrorResult<List<Summary>>("Summary not found.");
    }

    public async Task<ServiceResult<List<Summary>>> GetAllByCategoryAndSubcategory(string category, string subcategory, bool isAvaliable = false, CancellationToken cancellationToken = default)
    {
        var summaries = await _repository.GetAllByCategoryAndSubcategory(category, subcategory, isAvaliable, cancellationToken);

        if (summaries.Any())
        {
            return new()
            {
                Data = summaries,
                Success = true
            };
        }

        return MakeErrorResult<List<Summary>>("Summary not found.");
    }

    public async Task<ServiceResult<List<Summary>>> GetAllByOwnerId(string ownerId, bool isAvaliable = false, CancellationToken cancellationToken = default)
    {
        var summaries = await _repository.GetAllByOwnerId(ownerId, isAvaliable, cancellationToken);

        if (summaries.Any())
        {
            return new()
            {
                Data = summaries,
                Success = true
            };
        }

        return MakeErrorResult<List<Summary>>("Summary not found.");
    }

    public async Task<ServiceResult<List<Summary>>> GetAllBySubcategory(string subcategory, bool isAvaliable = false, CancellationToken cancellationToken = default)
    {
        var summaries = await _repository.GetAllBySubcategory(subcategory, isAvaliable, cancellationToken);

        if (summaries.Any())
        {
            return new()
            {
                Data = summaries,
                Success = true
            };
        }

        return MakeErrorResult<List<Summary>>("Summary not found.");
    }

    public async Task<ServiceResult<string>> RequestCreationToAI(SummaryCreationRequest request, CancellationToken cancellationToken = default)
    {
        var configurationResponse = await _configurationService.Get(request.ConfigurationId, cancellationToken);

        if (!configurationResponse.Success)
            return MakeErrorResult<string>("Configuration not founded");

        var configuration = configurationResponse.Data;

        var openAIRequest = $"{configuration.Summary.InitialInput} {request.Theme} {configuration.Summary.FinalInput}";

        var openAIResponse = await _openAIService.DoRequest(openAIRequest);

        if (string.IsNullOrEmpty(openAIResponse.Id))
            return MakeErrorResult<string>("Failed to get OPENAI response");

        var chatCompletionResult = await _chatCompletionsService.Save(openAIResponse, cancellationToken);

        if (!chatCompletionResult.Success)
            return MakeErrorResult<string>("Failed to save OpenAI response");

        var summary = new Summary
        {
            Id = Guid.NewGuid().ToString(),
            Category = request.Category,
            Subcategory = request.Subcategory,
            OwnerId = request.OwnerId,
            ConfigurationId = request.ConfigurationId,
            ChatId = chatCompletionResult.Data,
            Icon = request.Icon,
            CreatedDate = DateTime.UtcNow,
            Theme = request.Theme,
            Topics = MapTopicsFromResponse(openAIResponse)
        };

        var repositoryResponse = await _repository.Save(summary, cancellationToken);

        if (!repositoryResponse)
            return MakeErrorResult<string>("Failed to save summary");

        return new()
        {
            Success = true,
            Data = summary.Id
        };
    }

    public async Task<ServiceResult<string>> Save(SummaryRequest request, string newId = "", CancellationToken cancellationToken = default)
    {
        var summary = new Summary
        {
            Id = string.IsNullOrEmpty(newId) ? Guid.NewGuid().ToString() : newId,
            OriginId = request.OriginId,
            OwnerId = request.OwnerId,
            ConfigurationId = request.ConfigurationId,
            ChatId = request.ChatId,
            CreatedDate = DateTime.UtcNow,
            IsAvaliable = request.IsAvaliable,
            Category = request.Category,
            Subcategory = request.Subcategory,
            Theme = request.Theme,
            Icon = request.Icon,
            Topics = request.Topics
        };

        var success = await _repository.Save(summary, cancellationToken);

        if (!success)
            return MakeErrorResult<string>("Failed to save.");

        return new()
        {
            Success = true,
            Data = summary.Id
        };
    }

    public async Task<ServiceResult<string>> Update(string summaryId, SummaryRequest request, CancellationToken cancellationToken = default)
    {
        var summary = await _repository.Get(summaryId, cancellationToken);

        if (summary == null)
            return new()
            {
                Success = false,
                ErrorMessage = "Summary not found."
            };

        var updated = await _repository.Update(summaryId, request, cancellationToken);

        if (!updated)
            return MakeErrorResult<string>("Failed to update.");

        return new()
        {
            Success = true,
            Data = summaryId
        };
    }

    private static List<Topic> MapTopicsFromResponse(OpenAIResponse response)
    {
        var content = response.Choices.First().Message.Content;

        var objectFromResponse = JsonConvert.DeserializeObject<Summary>(content);

        if (objectFromResponse is not null)
            return objectFromResponse.Topics;
       

        return new();
    }

    private static ServiceResult<T> MakeErrorResult<T>(string message) => new()
    {
        Success = false,
        ErrorMessage = message,
        Data = default
    };
}