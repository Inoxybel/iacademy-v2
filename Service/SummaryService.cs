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

    public SummaryService(
        ISummaryRepository repository,
        IConfigurationService configurationService,
        IOpenAIService openAIService)
    {
        _repository = repository;
        _configurationService = configurationService;
        _openAIService = openAIService;
    }

    public async Task<ServiceResult<Summary>> Get(string id, CancellationToken cancellationToken = default)
    {
        var summary = await _repository.Get(id, cancellationToken);

        if (summary is null)
        {
            return new()
            {
                Success = false,
                ErrorMessage = "Summary not found."
            };
        }

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

        return new()
        {
            Success = false,
            ErrorMessage = "Summary not found."
        };
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

        return new()
        {
            Success = false,
            ErrorMessage = "Summary not found."
        };
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

        return new()
        {
            Success = false,
            ErrorMessage = "Summary not found."
        };
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

        return new()
        {
            Success = false,
            ErrorMessage = "Summary not found."
        };
    }

    public async Task<ServiceResult<string>> RequestCreationToAI(SummaryCreationRequest request, CancellationToken cancellationToken = default)
    {
        var configurationResponse = await _configurationService.Get(request.ConfigurationId, cancellationToken);

        if (!configurationResponse.Success)
            return new()
            {
                Success = false,
                ErrorMessage = "Configuration not founded"
            };

        var configuration = configurationResponse.Data;

        var openAIRequest = $"{configuration.Summary.InitialInput} {request.Theme} {configuration.Summary.FinalInput}";

        var openAIResponse = await _openAIService.DoRequest(openAIRequest);

        if (string.IsNullOrEmpty(openAIResponse.Id))
            return new()
            {
                Success = false,
                ErrorMessage = "Failed to get OPENAI response"
            };

        var summary = new Summary
        {
            Id = Guid.NewGuid().ToString(),
            Category = request.Category,
            Subcategory = request.Subcategory,
            OwnerId = request.OwnerId,
            ConfigurationId = request.ConfigurationId,
            CreatedDate = DateTime.UtcNow,
            Theme = request.Theme,
            Topics = MapTopicsFromResponse(openAIResponse)
        };

        var repositoryResponse = await _repository.Save(summary, cancellationToken);

        if (!repositoryResponse)
            return new()
            {
                Success = false,
                ErrorMessage = "Failed to save summary"
            };

        return new()
        {
            Success = true,
            Data = summary.Id
        };
    }

    public async Task<ServiceResult<string>> Save(string summaryId, SummaryRequest request, CancellationToken cancellationToken = default)
    {
        var summary = new Summary
        {
            Id = summaryId,
            OwnerId = request.OwnerId,
            ConfigurationId = request.ConfigurationId,
            CreatedDate = DateTime.UtcNow,
            IsAvaliable = request.IsAvaliable,
            Category = request.Category,
            Subcategory = request.Subcategory,
            Theme = request.Theme,
            Topics = request.Topics
        };

        var success = await _repository.Save(summary, cancellationToken);

        if (!success)
            return new()
            {
                Success = false,
                ErrorMessage = "Failed to save."
            };

        return new()
        {
            Success = true,
            Data = summaryId
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
            return new()
            {
                Success = false,
                ErrorMessage = "Failed to update."
            };

        return new()
        {
            Success = true,
            Data = summaryId
        };
    }

    private static List<Topic> MapTopicsFromResponse(OpenAIResponse response)
    {
        var completeResponse = response.Choices.First().Message.Content;

        var startIndex = completeResponse.IndexOf('{');
        var endIndex = completeResponse.LastIndexOf('}');

        if (startIndex != -1 && endIndex != -1)
        {
            var extractedJson = completeResponse.Substring(startIndex, endIndex - startIndex + 1);

            var objectFromResponse = JsonConvert.DeserializeObject<Summary>(extractedJson);

            if (objectFromResponse is not null)
                return objectFromResponse.Topics;
        }

        return new();
    }
}