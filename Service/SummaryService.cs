using Domain.DTO;
using Domain.DTO.Summary;
using Domain.Entities.Companies;
using Domain.Entities.Summary;
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
    private readonly ICompanyService _companyService;

    public SummaryService(
        ISummaryRepository repository,
        IConfigurationService configurationService,
        IOpenAIService openAIService,
        IChatCompletionsService chatCompletionsService,
        ICompanyService companyService)
    {
        _repository = repository;
        _configurationService = configurationService;
        _openAIService = openAIService;
        _chatCompletionsService = chatCompletionsService;
        _companyService = companyService;
    }

    public async Task<ServiceResult<Summary>> Get(string id, CancellationToken cancellationToken = default)
    {
        var summary = await _repository.Get(id, cancellationToken);

        if (summary is null)
            return ServiceResult<Summary>.MakeErrorResult("Summary not found.");

        return ServiceResult<Summary>.MakeSuccessResult(summary);
    }

    public async Task<ServiceResult<List<Summary>>> GetAllByOwnerId(
        string ownerId,
        bool isAvaliable = false,
        CancellationToken cancellationToken = default)
    {
        var summaries = await _repository.GetAllByOwnerId(ownerId, isAvaliable, cancellationToken);

        if (summaries.Any())
        {
            return ServiceResult<List<Summary>>.MakeSuccessResult(summaries);
        }

        return ServiceResult<List<Summary>>.MakeErrorResult("Summary not found.");
    }

    public async Task<ServiceResult<List<Summary>>> GetAllAvaliableByDocument(string ownerId, string document, string companyRef, bool isAvaliable = false, CancellationToken cancellationToken = default)
    {
        var summaryIdsFromCompanyResponse = await GetAllAvaliableSummaryIdForUser(document, companyRef, cancellationToken);

        if (!summaryIdsFromCompanyResponse.Success)
            return ServiceResult<List<Summary>>.MakeErrorResult("There are no avaliable summaries for this user.");

        var summaryIds = summaryIdsFromCompanyResponse.Data;

        var summaries = await _repository.GetAllByIds(summaryIds, isAvaliable, cancellationToken);

        if (summaries.Any())
        {
            var enrolledSummaryIds = await IsEnrolled(summaryIds, ownerId, cancellationToken);

            if(enrolledSummaryIds.Any())
            {
                var filtredAvaliableSummaries = summaries
                    .Where(s => !enrolledSummaryIds.Contains(s.Id))
                    .ToList();
   
                return ServiceResult<List<Summary>>.MakeSuccessResult(filtredAvaliableSummaries);
            }

            var avaliableSummaries = summaries
                .Where(s => !enrolledSummaryIds.Contains(s.OriginId))
                .ToList();

            return ServiceResult<List<Summary>>.MakeSuccessResult(avaliableSummaries);
        }

        return ServiceResult<List<Summary>>.MakeErrorResult("Summaries not found.");
    }

    public async Task<ServiceResult<List<Summary>>> GetAllByCategory(
        string category,
        string document, 
        string companyRef,
        bool isAvaliable = false, 
        CancellationToken cancellationToken = default)
    {
        var summaryIdsFromCompanyResponse = await GetAllAvaliableSummaryIdForUser(document, companyRef, cancellationToken);

        if (summaryIdsFromCompanyResponse.Success)
            return ServiceResult<List<Summary>>.MakeErrorResult("There are no avaliable summaries for this user.");

        var summaryIds = summaryIdsFromCompanyResponse.Data;

        var summaries = await _repository.GetAllByCategory(summaryIds, category, isAvaliable, cancellationToken);

        if (summaries.Any())
        {
            return ServiceResult<List<Summary>>.MakeSuccessResult(summaries);
        }

        return ServiceResult<List<Summary>>.MakeErrorResult("Summary not found.");
    }

    public async Task<ServiceResult<List<Summary>>> GetAllByCategoryAndSubcategory(
        string category,
        string subcategory, 
        string document, 
        string companyRef,
        bool isAvaliable = false, 
        CancellationToken cancellationToken = default)
    {
        var summaryIdsFromCompanyResponse = await GetAllAvaliableSummaryIdForUser(document, companyRef, cancellationToken);

        if (summaryIdsFromCompanyResponse.Success)
            return ServiceResult<List<Summary>>.MakeErrorResult("There are no avaliable summaries for this user.");

        var summaryIds = summaryIdsFromCompanyResponse.Data;

        var summaries = await _repository.GetAllByCategoryAndSubcategory(summaryIds, category, subcategory, isAvaliable, cancellationToken);

        if (summaries.Any())
        {
            return ServiceResult<List<Summary>>.MakeSuccessResult(summaries);
        }

        return ServiceResult<List<Summary>>.MakeErrorResult("Summaries not found.");
    }

    public async Task<ServiceResult<List<Summary>>> GetAllBySubcategory(
        string subcategory,
        string document,
        string companyRef, 
        bool isAvaliable = false, 
        CancellationToken cancellationToken = default)
    {
        var summaryIdsFromCompanyResponse = await GetAllAvaliableSummaryIdForUser(document, companyRef, cancellationToken);

        if (summaryIdsFromCompanyResponse.Success)
            return ServiceResult<List<Summary>>.MakeErrorResult("There are no avaliable summaries for this user.");

        var summaryIds = summaryIdsFromCompanyResponse.Data;

        var summaries = await _repository.GetAllBySubcategory(summaryIds, subcategory, isAvaliable, cancellationToken);

        if (summaries.Any())
        {
            return ServiceResult<List<Summary>>.MakeSuccessResult(summaries);
        }

        return ServiceResult<List<Summary>>.MakeErrorResult("Summary not found.");
    }

    public async Task<ServiceResult<string>> RequestCreationToAI(SummaryCreationRequest request, CancellationToken cancellationToken = default)
    {
        var configurationResponse = await _configurationService.Get(request.ConfigurationId, cancellationToken);

        if (!configurationResponse.Success)
            return ServiceResult<string>.MakeErrorResult("Configuration not founded");

        var configuration = configurationResponse.Data;

        var openAIRequest = $"{configuration.Summary.InitialInput} {request.Theme} {configuration.Summary.FinalInput}";

        var openAIResponse = await _openAIService.DoRequest(configuration.Summary, openAIRequest);

        if (string.IsNullOrEmpty(openAIResponse.Id))
            return ServiceResult<string>.MakeErrorResult("Failed to get OPENAI response");

        var chatCompletionResult = await _chatCompletionsService.Save(openAIResponse, cancellationToken);

        if (!chatCompletionResult.Success)
            return ServiceResult<string>.MakeErrorResult("Failed to save OpenAI response");

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
            return ServiceResult<string>.MakeErrorResult("Failed to save summary");

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
            return ServiceResult<string>.MakeErrorResult("Failed to save.");

        return new()
        {
            Success = true,
            Data = summary.Id
        };
    }

    public async Task<ServiceResult<string>> Update(string summaryId, SummaryRequest request, CancellationToken cancellationToken = default)
    {
        var summary = await _repository.Get(summaryId, cancellationToken);

        if (summary is null)
            return ServiceResult<string>.MakeErrorResult("Summary not found.");

        var updated = await _repository.Update(summaryId, request, cancellationToken);

        if (!updated)
            return ServiceResult<string>.MakeErrorResult("Failed to update.");

        return ServiceResult<string>.MakeSuccessResult(summaryId);
    }

    public async Task<ServiceResult<bool>> UpdateProgress(string summaryId, string subtopicIndex, CancellationToken cancellationToken)
    {
        var summary = await _repository.Get(summaryId, cancellationToken);

        if (summary is null)
            return ServiceResult<bool>.MakeErrorResult("Summary not found.");


        var subtopic = summary.Topics
            .SelectMany(t => t.Subtopics)
            .FirstOrDefault(s => s.Index == subtopicIndex);

        if (subtopic != null)
        {
            subtopic.IsCompleted = true;

            SummaryRequest summaryUpdateRequest = summary;

            var updateResult = await _repository.Update(summaryId, summaryUpdateRequest, cancellationToken);

            if (updateResult)
                return ServiceResult<bool>.MakeSuccessResult(true);

            return ServiceResult<bool>.MakeErrorResult("Fail to update summary.");
        }

        return ServiceResult<bool>.MakeErrorResult("Fail to get subtopicIndex");
    }

    public async Task<bool> IsEnrolled(string summaryId, string ownerId, CancellationToken cancellationToken = default) =>
        await _repository.IsEnrolled(summaryId, ownerId, cancellationToken);

    public async Task<List<string>> IsEnrolled(List<string> summaryIds, string ownerId, CancellationToken cancellationToken = default) =>
        await _repository.IsEnrolled(summaryIds, ownerId, cancellationToken);

    private async Task<ServiceResult<List<string>>> GetAllAvaliableSummaryIdForUser(string document, string companyRef, CancellationToken cancellationToken)
    {
        var company = await _companyService.GetByRef(companyRef, cancellationToken);

        if (company is null)
            return ServiceResult<List<string>>.MakeErrorResult("Company not found.");

        var groups = company.GetGroupByUserDocument(document);

        if (groups is null || !groups.Any())
            return ServiceResult<List<string>>.MakeErrorResult("Contact your company.");

        return ServiceResult<List<string>>.MakeSuccessResult(GetSummaryIdsFromGroup(groups));
    }

    private static List<string> GetSummaryIdsFromGroup(List<CompanyGroup> groups) =>
        groups
            .SelectMany(group => group.AuthorizedTrainingIds)
            .Distinct()
            .ToList();

    private static List<Topic> MapTopicsFromResponse(OpenAIResponse response)
    {
        var content = response.Choices.First().Message.Content;

        var objectFromResponse = JsonConvert.DeserializeObject<Summary>(content);

        if (objectFromResponse is not null)
            return objectFromResponse.Topics;

        return new();
    }

    public async Task<bool> ShouldGeneratePendency(string summaryId, string ownerId, CancellationToken cancellationToken = default) =>
        await _repository.ShouldGeneratePendency(summaryId, ownerId, cancellationToken);
}