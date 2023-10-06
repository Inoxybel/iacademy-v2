using CrossCutting.Extensions;
using Domain.DTO;
using Domain.DTO.Content;
using Domain.DTO.Summary;
using Domain.Entities.Configuration;
using Domain.Entities.Contents;
using Domain.Entities.Exercise;
using Domain.Entities.Summary;
using Domain.Infra;
using Domain.Services;
using Service.Integrations.OpenAI.DTO;
using Service.Integrations.OpenAI.Interfaces;

namespace Service;

public class ContentService : IContentService
{
    private readonly IContentRepository _contentRepository;
    private readonly IOpenAIService _openAIService;
    private readonly IConfigurationService _configurationService;
    private readonly IGeneratorService _exerciseGeneratorService;
    private readonly ISummaryService _summaryService;
    private readonly IExerciseService _exerciseService;
    private readonly IChatCompletionsService _chatCompletionsService;
    private readonly ICompanyService _companyService;

    public ContentService(
        IContentRepository contentRepository,
        ISummaryService summaryService,
        IGeneratorService exerciseGeneratorService,
        IOpenAIService openAIService,
        IConfigurationService configurationService,
        IExerciseService exerciseService,
        IChatCompletionsService chatCompletionsService,
        ICompanyService companyService)
    {
        _contentRepository = contentRepository;
        _summaryService = summaryService;
        _exerciseGeneratorService = exerciseGeneratorService;
        _openAIService = openAIService;
        _configurationService = configurationService;
        _exerciseService = exerciseService;
        _chatCompletionsService = chatCompletionsService;
        _companyService = companyService;
    }

    public async Task<ServiceResult<Content>> Get(string id, CancellationToken cancellationToken = default)
    {
        var content = await _contentRepository.Get(id, cancellationToken);

        if (content == null)
            return ServiceResult<Content>.MakeErrorResult("Content not found.");

        return ServiceResult<Content>.MakeSuccessResult(content);
    }

    public async Task<ServiceResult<List<Content>>> GetAllBySummaryId(string summaryId, CancellationToken cancellationToken = default)
    {
        var contents = await _contentRepository.GetAllBySummaryId(summaryId, cancellationToken);

        if (contents == null || !contents.Any())
            return ServiceResult<List<Content>>.MakeErrorResult("No contents found.");

        return ServiceResult<List<Content>>.MakeSuccessResult(contents);
    }

    public async Task<ServiceResult<string>> Save(ContentRequest request, CancellationToken cancellationToken = default)
    {
        var content = new Content()
        {
            Id = Guid.NewGuid().ToString(),
            OwnerId = request.OwnerId,
            SummaryId = request.SummaryId,
            ConfigurationId = request.ConfigurationId,
            ExerciseId = request.ExerciseId,
            Theme = request.Theme,
            SubtopicIndex = request.SubtopicIndex,
            Title = request.Title,
            Body = request.Body,
            CreatedDate = DateTime.UtcNow
        };

        var repositoryResponse = await _contentRepository.Save(content, cancellationToken);

        if (string.IsNullOrEmpty(repositoryResponse))
            return ServiceResult<string>.MakeErrorResult("Failed to save content.");

        return ServiceResult<string>.MakeSuccessResult(repositoryResponse);
    }

    public async Task<ServiceResult<List<string>>> SaveAll(List<ContentRequest> contents, CancellationToken cancellationToken = default)
    {
        if (!contents.Any())
            return ServiceResult<List<string>>.MakeErrorResult("No contents sended");

        var contentsToSave = new List<Content>();

        foreach (var content in contents)
        {
            contentsToSave.Add(
                MakeNewContentFromContentRequest(content)
            );
        }

        var repositoryResponse = await _contentRepository.SaveAll(contentsToSave, cancellationToken);

        if (!repositoryResponse.Any())
            return ServiceResult<List<string>>.MakeErrorResult("Failed to save all contents.");

        return ServiceResult<List<string>>.MakeSuccessResult(repositoryResponse);
    }



    public async Task<ServiceResult<bool>> Update(string contentId, ContentRequest request, CancellationToken cancellationToken = default)
    {
        var isSuccess = await _contentRepository.Update(contentId, request, cancellationToken);

        if (!isSuccess)
            return ServiceResult<bool>.MakeErrorResult("Failed to update content.");

        return ServiceResult<bool>.MakeSuccessResult(true);
    }

    public async Task<ServiceResult<bool>> UpdateAll(string summaryId, List<Content> contents, CancellationToken cancellationToken = default)
    {
        var isSuccess = await _contentRepository.UpdateAll(summaryId, contents, cancellationToken);

        if (!isSuccess)
            return ServiceResult<bool>.MakeErrorResult($"Failed to update contents for summary ID {summaryId}.");

        return ServiceResult<bool>.MakeSuccessResult(true);
    }

    public async Task<ServiceResult<string>> MakeContent(string summaryId, AIContentCreationRequest request, CancellationToken cancellationToken = default)
    {
        Content newContent = new();

        var getSummaryResult = await _summaryService.Get(summaryId, cancellationToken);

        if (!getSummaryResult.Success)
            return ServiceResult<string>.MakeErrorResult("Error to find summary");

        var summary = getSummaryResult.Data;

        var getConfigurationResult = await _configurationService.Get(summary.ConfigurationId, cancellationToken);

        if (!getConfigurationResult.Success)
            return ServiceResult<string>.MakeErrorResult("Error to find configuration");

        var configuration = getConfigurationResult.Data;

        var subtopic = summary.Topics.SelectMany(t => t.Subtopics).FirstOrDefault(s => s.Index == request.SubtopicIndex);

        if (subtopic is null)
            return ServiceResult<string>.MakeErrorResult("Error to find index on topic");

        var chatCompletions = await _chatCompletionsService.Get(summary.ChatId, cancellationToken);

        OpenAIResponse openAIResponse;

        if (chatCompletions.Success)
        {
            openAIResponse = await _openAIService.DoRequest(chatCompletions.Data, configuration.FirstContent, subtopic.Index);

            if (!string.IsNullOrEmpty(openAIResponse.Id))
            {
                chatCompletions.Data.Choices.Add(new()
                {
                    Index = 2,
                    Message = new()
                    {
                        Role = "assistant",
                        Content = openAIResponse.Choices.First().Message.Content
                    }

                });
            }
        }
        else
        {            
            var requestString = MakeOpenAIRequest(summary.Theme, subtopic.Title);

            openAIResponse = await _openAIService.DoRequest(configuration.FirstContent, requestString);
        }

        if (string.IsNullOrEmpty(openAIResponse.Id))
            return ServiceResult<string>.MakeErrorResult("Error to get OpenAI content create response");

        var summaryContent = openAIResponse.Choices.First().Message.Content.Deserialize<SummaryContentsDTO>();

        if (summaryContent.Subtopic is null)
            summaryContent = openAIResponse.Choices.First().Message.Content.Deserialize<SummaryContentsDTO>(true);

        newContent = MakeNewContent(summary, summaryContent.Subtopic, request.SubtopicIndex);

        var saveChatResult = await _chatCompletionsService.Save(openAIResponse, cancellationToken);

        if (saveChatResult.Success)
            newContent.ChatId = saveChatResult.Data;

        var saveContentResult = await _contentRepository.Save(newContent, cancellationToken);

        if (string.IsNullOrEmpty(saveContentResult))
            return ServiceResult<string>.MakeErrorResult("Error to save content");

        var makeExerciseResult = await _exerciseGeneratorService.MakeExercise(newContent, configuration.Exercise, configuration.Id, cancellationToken);

        if (!makeExerciseResult.Success)
            return ServiceResult<string>.MakeErrorResult("Error to make exercise");

        newContent.ExerciseId = makeExerciseResult.Data;

        saveContentResult = await _contentRepository.Save(newContent, cancellationToken);

        if (string.IsNullOrEmpty(saveContentResult))
            return ServiceResult<string>.MakeErrorResult("Error to update content");

        subtopic.ContentId = newContent.Id;
        subtopic.ExerciseId = makeExerciseResult.Data;

        var summaryUpdateRequest = new SummaryRequest()
        {
            OwnerId = summary.OwnerId,
            ConfigurationId = summary.ConfigurationId,
            IsAvaliable = summary.IsAvaliable,
            Category = summary.Category,
            Subcategory = summary.Subcategory,
            Theme = summary.Theme,
            Icon = summary.Icon,
            Topics = summary.Topics
        };

        var updateSummaryResult = await _summaryService.Update(summaryId, summaryUpdateRequest, cancellationToken);

        if (!updateSummaryResult.Success)
            return ServiceResult<string>.MakeErrorResult("Error to update summary");

        return ServiceResult<string>.MakeSuccessResult(newContent.Id);
    }

    public async Task<ServiceResult<string>> MakeAlternativeContent(string contentId, SubcontentRecreationRequest request, CancellationToken cancellationToken = default)
    {
        var content = await _contentRepository.Get(contentId, cancellationToken);

        if (content is null)
            return ServiceResult<string>.MakeErrorResult("Content not found");

        var getConfigurationResult = await _configurationService.Get(content.ConfigurationId, cancellationToken);

        if (!getConfigurationResult.Success)
            return ServiceResult<string>.MakeErrorResult("Error to find configuration");

        var configuration = getConfigurationResult.Data;

        var contents = content.Body.Contents;

        if (ValidateIndexOutOfRange(request.SubcontentIndex, contents.Count))
            return ServiceResult<string>.MakeErrorResult("Invalid index");

        var openAIResponse = new OpenAIResponse();

        if (string.IsNullOrEmpty(content.ChatId))
        {
            openAIResponse = await CommonRequestOpenAPI(contents, request, configuration);
        }
        else
        {
            var chatCompletionResponse = await _chatCompletionsService.Get(content.ChatId, cancellationToken);

            if(chatCompletionResponse.Success)
            {

                openAIResponse = await _openAIService.DoRequest(chatCompletionResponse.Data, configuration.NewContent, string.Empty);
            }
            else
            {
                openAIResponse = await CommonRequestOpenAPI(contents, request, configuration);
            }
        } 

        if (string.IsNullOrEmpty(openAIResponse.Id))
            return ServiceResult<string>.MakeErrorResult("Error to get OpenAI response");

        var recreatedContent = openAIResponse.Choices.First().Message.Content.Deserialize<AISubcontentRecreatedResponse>();

        if (string.IsNullOrEmpty(recreatedContent.NewContent))
            recreatedContent = openAIResponse.Choices.First().Message.Content.Deserialize<AISubcontentRecreatedResponse>(true);

        if (recreatedContent is null || string.IsNullOrEmpty(recreatedContent.NewContent))
            return ServiceResult<string>.MakeErrorResult("Error to deserialize generated content.");

        var newContentList = MakeNewContentList(content.Body.Contents, request.SubcontentIndex, recreatedContent.NewContent);

        content.Body.Contents = newContentList;

        var saveContentResult = await _contentRepository.Save(content, cancellationToken);

        if (string.IsNullOrEmpty(saveContentResult))
            return ServiceResult<string>.MakeErrorResult("Error to update content");

        return ServiceResult<string>.MakeSuccessResult(contentId);
    }

    public async Task<ServiceResult<string>> CopyContentsToEnrollUser(SummaryMatriculationRequest request, string ownerId, string companyRef, string document, CancellationToken cancellationToken = default)
    {
        var summaryServiceResponse = await _summaryService.Get(request.SummaryId, cancellationToken);

        if (!summaryServiceResponse.Success)
            return ServiceResult<string>.MakeErrorResult("Summary not found.");

        var summary = summaryServiceResponse.Data;

        var company = await _companyService.GetByRef(companyRef, cancellationToken);

        if (company is null)
            return ServiceResult<string>.MakeErrorResult("Company not found.");

        var groups = company.GetGroupByUserDocument(document);

        if (groups is null || !groups.Any())
            return ServiceResult<string>.MakeErrorResult("Contact your company.");

        if (!groups.Any(g => g.AuthorizedTrainingIds.Contains(request.SummaryId)))
            return ServiceResult<string>.MakeErrorResult("This training is not enabled for your company.");

        if (!summary.OwnerId.Equals(company.Cnpj))
            return ServiceResult<string>.MakeErrorResult("This summary is not master.");

        var isEnrolled = await _summaryService.IsEnrolled(request.SummaryId, ownerId, cancellationToken);

        if (isEnrolled)
            return ServiceResult<string>.MakeErrorResult("Is already enrolled.");

        summary.Id = Guid.NewGuid().ToString();
        summary.OriginId = request.SummaryId;
        summary.OwnerId = ownerId;

        var contentIds = summary.Topics
            .SelectMany(topic => topic.Subtopics)
            .Where(subtopic => subtopic != null && !string.IsNullOrEmpty(subtopic.ContentId))
            .Select(subtopic => subtopic.ContentId)
            .ToList();

        if (!contentIds.Any())
            return ServiceResult<string>.MakeErrorResult("Error to get content ids.");

        var baseContents = await _contentRepository.GetAllByIds(contentIds, cancellationToken);

        if (!baseContents.Any())
            return ServiceResult<string>.MakeErrorResult("Error to get contents.");

        var exerciseIds = baseContents
            .Where(content => !string.IsNullOrEmpty(content.ExerciseId))
            .Select(content => content.ExerciseId)
            .ToList();

        if (!exerciseIds.Any())
            return ServiceResult<string>.MakeErrorResult("Error to get exercise ids.");

        var baseExercises = await _exerciseService.GetAllByIds(exerciseIds, cancellationToken);

        if(!baseExercises.Success)
            return ServiceResult<string>.MakeErrorResult("Error to get exercises.");

        summary = await CreateNewReferences(baseContents, baseExercises.Data, summary);

        var newContentsSaveResult = await _contentRepository.SaveAll(baseContents, cancellationToken);

        if(!newContentsSaveResult.Any())
            return ServiceResult<string>.MakeErrorResult("Error to save new contents.");

        var newExercisesSaveResult = await _exerciseService.SaveAll(baseExercises.Data, cancellationToken);

        if (!newExercisesSaveResult.Success)
            return ServiceResult<string>.MakeErrorResult("Error to save new exercises.");

        var summarySaveRequest = MakeSummaryRequest(summary);

        var summaryRepositoryResponse = await _summaryService.Save(summarySaveRequest, summary.Id, cancellationToken);

        if(!summaryRepositoryResponse.Success)
            return ServiceResult<string>.MakeErrorResult("Fail to enroll user, try again.");

        return ServiceResult<string>.MakeSuccessResult(summaryRepositoryResponse.Data);
    }

    private async Task<OpenAIResponse> CommonRequestOpenAPI(List<Subcontent> contents, SubcontentRecreationRequest request, Configuration configuration)
    {
        var partToRegenerate = GetSubcontentHistory(contents, request);

        if (partToRegenerate?.Content is null)
            return new();

        return await _openAIService.DoRequest(configuration.NewContent, partToRegenerate.Content);
    }

    private static SubcontentHistory GetSubcontentHistory(List<Subcontent> contents, SubcontentRecreationRequest request) =>
        contents[request.SubcontentIndex].SubcontentHistory.FirstOrDefault(h => h.DisabledDate == DateTime.MinValue)!;

    private static bool ValidateIndexOutOfRange(int index, int limit) =>
     index < 0 || index >= limit;

    private static Content MakeNewContent(Summary summary, SummarySubtopicDTO subtopic, string subtopicIndex)
    {
        return new()
        {
            Id = Guid.NewGuid().ToString(),
            OwnerId = summary.OwnerId,
            ConfigurationId = summary.ConfigurationId,
            SummaryId = summary.Id,
            Theme = summary.Theme,
            SubtopicIndex = subtopicIndex,
            Title = subtopic.Title,
            CreatedDate = DateTime.UtcNow,
            Body = new Body()
            {
                Contents = subtopic.Content.Select(c =>
                    new Subcontent()
                    {
                        SubcontentHistory = new List<SubcontentHistory>()
                        {
                            new SubcontentHistory()
                            {
                                Content = c,
                                CreatedDate = DateTime.UtcNow,
                                DisabledDate = DateTime.MinValue
                            }
                        }
                    }
                ).ToList()
            }
        };
    }

    private static Content MakeNewContentFromContentRequest(ContentRequest content) => new()
    {
        Id = Guid.NewGuid().ToString(),
        OwnerId = content.OwnerId,
        SummaryId = content.SummaryId,
        ConfigurationId = content.ConfigurationId,
        ExerciseId = content.ExerciseId,
        Theme = content.Theme,
        SubtopicIndex = content.SubtopicIndex,
        Title = content.Title,
        Body = content.Body,
        CreatedDate = DateTime.UtcNow
    };

    private static SummaryRequest MakeSummaryRequest(Summary summary) => new()
    {
        OriginId = summary.OriginId,
        OwnerId = summary.OwnerId,
        ConfigurationId = summary.ConfigurationId,
        IsAvaliable = true,
        Category = summary.Category,
        Subcategory = summary.Subcategory,
        Theme = summary.Theme,
        Topics = summary.Topics,
        Icon = summary.Icon,
        ChatId = summary.ChatId
    };

    private static async Task<Summary> CreateNewReferences(List<Content> baseContents, List<Exercise> baseExercises, Summary summary)
    {
        var contentIdMap = baseContents.Where(c => c is not null).ToDictionary(
            content => content.Id,
            content => Guid.NewGuid().ToString()
        );

        var exerciseIdMap = baseExercises.Where(e => e is not null).ToDictionary(
            exercise => exercise.Id,
            exercise => Guid.NewGuid().ToString()
        );

        var reverseExerciseIdMap = baseExercises.Where(e => e is not null && e.ContentId is not null).ToDictionary(
            exercise => exercise.Id,
            exercise => contentIdMap.ContainsKey(exercise.ContentId) ? contentIdMap[exercise.ContentId] : null
        );

        baseContents.ForEach(content =>
        {
            if (content is not null)
            {
                content.ExerciseId = content.ExerciseId is not null ? exerciseIdMap[content.ExerciseId] : null;
                content.OwnerId = summary.OwnerId;
                content.SummaryId = summary.Id;
                content.Id = contentIdMap[content.Id];
                content.CreatedDate = DateTime.UtcNow;
                content.UpdatedDate = DateTime.MinValue;
            }
        });

        baseExercises.ForEach(exercise =>
        {
            if (exercise is not null)
            {
                exercise.ContentId = reverseExerciseIdMap.ContainsKey(exercise.Id) ? reverseExerciseIdMap[exercise.Id] : null;
                exercise.OwnerId = summary.OwnerId;
                exercise.Id = exerciseIdMap[exercise.Id];
            }
        });

        summary.Topics.ForEach(topic =>
        {
            topic.Subtopics.ForEach(subtopic =>
            {
                if (subtopic.ContentId is not null && contentIdMap.ContainsKey(subtopic.ContentId))
                    subtopic.ContentId = contentIdMap[subtopic.ContentId];

                if (subtopic.ExerciseId is not null && exerciseIdMap.ContainsKey(subtopic.ExerciseId))
                    subtopic.ExerciseId = exerciseIdMap[subtopic.ExerciseId];
            });
        });

        return summary;
    }

    private static List<Subcontent> MakeNewContentList(List<Subcontent> contents, int subtopicIndex, string newContent)
    {
        var contentToDesactive = contents[subtopicIndex].SubcontentHistory.Find(b => b.DisabledDate == DateTime.MinValue);

        contentToDesactive.DisabledDate = DateTime.UtcNow;

        contents[subtopicIndex].SubcontentHistory.Remove(contentToDesactive);

        var newContentList = new List<SubcontentHistory>
        {
            new()
            {
                Content = newContent,
                CreatedDate = DateTime.UtcNow,
                DisabledDate = DateTime.MinValue
            },
            contentToDesactive
        };

        newContentList.AddRange(contents[subtopicIndex].SubcontentHistory.FindAll(b => b.DisabledDate != DateTime.MinValue));

        contents[subtopicIndex].SubcontentHistory = newContentList;

        return contents;
    }

    private static string MakeOpenAIFirstContentRequest(InputProperties configuration, string topicIndex)
    {
        var request = $"{configuration.InitialInput}: {topicIndex} {configuration.FinalInput}";

        return request;
    }

    private static string MakeOpenAIRequest(string theme, string title)
    {
        var request = $"{theme} - {title}";

        return request;
    }
}