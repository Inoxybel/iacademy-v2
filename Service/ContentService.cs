using CrossCutting.Extensions;
using Domain.DTO;
using Domain.DTO.Content;
using Domain.DTO.Summary;
using Domain.Entities;
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

    public ContentService(
        IContentRepository contentRepository,
        ISummaryService summaryService,
        IGeneratorService exerciseGeneratorService,
        IOpenAIService openAIService,
        IConfigurationService configurationService,
        IExerciseService exerciseService,
        IChatCompletionsService chatCompletionsService)
    {
        _contentRepository = contentRepository;
        _summaryService = summaryService;
        _exerciseGeneratorService = exerciseGeneratorService;
        _openAIService = openAIService;
        _configurationService = configurationService;
        _exerciseService = exerciseService;
        _chatCompletionsService = chatCompletionsService;
    }

    public async Task<ServiceResult<Content>> Get(string id, CancellationToken cancellationToken = default)
    {
        var content = await _contentRepository.Get(id, cancellationToken);

        if (content == null)
            return new ServiceResult<Content>
            {
                Success = false,
                ErrorMessage = "Content not found."
            };

        return new()
        {
            Success = true,
            Data = content
        };
    }

    public async Task<ServiceResult<List<Content>>> GetAllBySummaryId(string summaryId, CancellationToken cancellationToken = default)
    {
        var contents = await _contentRepository.GetAllBySummaryId(summaryId, cancellationToken);

        if (contents == null || !contents.Any())
            return new ServiceResult<List<Content>>
            {
                Success = false,
                ErrorMessage = "No contents found."
            };

        return new()
        {
            Success = true,
            Data = contents
        };
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
            return new()
            {
                Success = false,
                ErrorMessage = "Failed to save content."
            };

        return new()
        {
            Success = true,
            Data = repositoryResponse
        };
    }

    public async Task<ServiceResult<List<string>>> SaveAll(List<ContentRequest> contents, CancellationToken cancellationToken = default)
    {
        if (!contents.Any())
            return new()
            {
                Success = false,
                ErrorMessage = "No contents sended"
            };

        var contentsToSave = new List<Content>();

        foreach (var content in contents)
        {
            contentsToSave.Add(new()
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
            });
        }

        var repositoryResponse = await _contentRepository.SaveAll(contentsToSave, cancellationToken);

        if (!repositoryResponse.Any())
            return new()
            {
                Success = false,
                ErrorMessage = "Failed to save all contents."
            };

        return new()
        {
            Success = true,
            Data = repositoryResponse
        };
    }

    public async Task<ServiceResult<bool>> Update(string contentId, ContentRequest request, CancellationToken cancellationToken = default)
    {
        var isSuccess = await _contentRepository.Update(contentId, request, cancellationToken);

        if (!isSuccess)
            return new ServiceResult<bool>
            {
                Success = false,
                ErrorMessage = "Failed to update content."
            };

        return new()
        {
            Success = true,
            Data = true
        };
    }

    public async Task<ServiceResult<bool>> UpdateAll(string summaryId, List<Content> contents, CancellationToken cancellationToken = default)
    {
        var isSuccess = await _contentRepository.UpdateAll(summaryId, contents, cancellationToken);

        if (!isSuccess)
            return new()
            {
                Success = false,
                ErrorMessage = $"Failed to update contents for summary ID {summaryId}."
            };

        return new()
        {
            Success = true,
            Data = true
        };
    }

    public async Task<ServiceResult<List<string>>> MakeContent(string summaryId, AIContentCreationRequest request, CancellationToken cancellationToken = default)
    {
        Content newContent = new();

        var getSummaryResult = await _summaryService.Get(summaryId, cancellationToken);

        if (!getSummaryResult.Success)
            return MakeErrorResult<List<string>>("Error to find summary");

        var summary = getSummaryResult.Data;

        var getConfigurationResult = await _configurationService.Get(summary.ConfigurationId, cancellationToken);

        if (!getConfigurationResult.Success)
            return MakeErrorResult<List<string>>("Error to find configuration");

        var configuration = getConfigurationResult.Data;

        var topic = summary.Topics.Find(t => t.Index == request.TopicIndex);

        if (topic is null)
            return MakeErrorResult<List<string>>("Error to find index on topic");

        var chatCompletions = await _chatCompletionsService.Get(summary.ChatId, cancellationToken);

        OpenAIResponse openAIResponse;

        if (chatCompletions.Success)
        {
            var requestString = MakeOpenAIFirstContentRequest(configuration.FirstContent, request.TopicIndex);

            openAIResponse = await _openAIService.DoRequest(chatCompletions.Data, requestString);
        }
        else
        {
            var topicSummary = $"{topic.Index} {topic.Title}: {string.Join(", ", topic.Subtopics.Select(st => st.Index + " " + st.Title))}";
            
            var requestString = MakeOpenAIRequest(configuration.FirstContent, summary.Theme, string.Empty, topicSummary);

            openAIResponse = await _openAIService.DoRequest(requestString);
        }

        if (string.IsNullOrEmpty(openAIResponse.Id))
            return MakeErrorResult<List<string>>("Error to get OpenAI content create response");

        var newContents = openAIResponse.Choices.First().Message.Content.Deserialize<SummaryContentsDTO>();

        if (!newContents.IsValid())
            newContents = openAIResponse.Choices.First().Message.Content.Deserialize<SummaryContentsDTO>(true);

        var idList = new List<string>();

        foreach (var subtopic in newContents.Subtopics)
        {
            newContent = new Content()
            {
                Id = Guid.NewGuid().ToString(),
                OwnerId = summary.OwnerId,
                ConfigurationId = summary.ConfigurationId,
                SummaryId = summary.Id,
                Theme = summary.Theme,
                SubtopicIndex = subtopic.Index,
                Title = subtopic.Title,
                CreatedDate = DateTime.UtcNow,
                Body = new List<Body>()
                    {
                        new Body()
                        {
                            Content = subtopic.Content,
                            CreatedDate = DateTime.UtcNow
                        }
                    }
            };

            var saveContentResult = await _contentRepository.Save(newContent, cancellationToken);

            if (string.IsNullOrEmpty(saveContentResult))
                return MakeErrorResult<List<string>>("Error to save content");

            var makeExerciseResult = await _exerciseGeneratorService.MakeExercise(newContent.Id, cancellationToken);

            if (!makeExerciseResult.Success)
                return MakeErrorResult<List<string>>("Error to make exercise");

            newContent.ExerciseId = makeExerciseResult.Data;

            saveContentResult = await _contentRepository.Save(newContent, cancellationToken);

            if (string.IsNullOrEmpty(saveContentResult))
                return MakeErrorResult<List<string>>("Error to update content");

            var subtopicToUpdade = summary.Topics
                .SelectMany(topic => topic.Subtopics)
                .FirstOrDefault(sub => sub.Index == subtopic.Index);

            if (subtopicToUpdade is not null)
                subtopicToUpdade.ContentId = newContent.Id;

            idList.Add(newContent.Id);
        }

        var summaryUpdateRequest = new SummaryRequest()
        {
            OwnerId = summary.OwnerId,
            ConfigurationId = summary.ConfigurationId,
            IsAvaliable = summary.IsAvaliable,
            Category = summary.Category,
            Subcategory = summary.Subcategory,
            Theme = summary.Theme,
            Topics = summary.Topics
        };

        var updateSummaryResult = await _summaryService.Update(summaryId, summaryUpdateRequest, cancellationToken);

        if (!updateSummaryResult.Success)
            return MakeErrorResult<List<string>>("Error to update summary");

        return new()
        {
            Success = true,
            Data = idList
        };
    }

    public async Task<ServiceResult<string>> MakeAlternativeContent(string contentId, CancellationToken cancellationToken = default)
    {
        var content = await _contentRepository.Get(contentId, cancellationToken);

        if (content is null)
            return MakeErrorResult<string>("Content not found");

        var getConfigurationResult = await _configurationService.Get(content.ConfigurationId, cancellationToken);

        if (!getConfigurationResult.Success)
            return MakeErrorResult<string>("Error to find configuration");

        var configuration = getConfigurationResult.Data;

        var resultGetLastContent = content.Body.First(c => c.DisabledDate == DateTime.MinValue)?.Content;
        var lastContent = resultGetLastContent ?? string.Empty;

        var requestString = MakeOpenAIRequest(configuration.NewContent, content.Theme, content.Title, lastContent);

        var openAIResponse = await _openAIService.DoRequest(requestString);

        if (string.IsNullOrEmpty(openAIResponse.Id))
            return MakeErrorResult<string>("Error to get OpenAI response");

        var newContent = openAIResponse.Choices.First().Message.Content.Deserialize<Content>();

        if (!newContent.Body.Any())
            newContent = openAIResponse.Choices.First().Message.Content.Deserialize<Content>(true);

        var newBody = MakeNewContentList(content.Body, newContent.Body);

        content.Body = newBody;

        var saveContentResult = await _contentRepository.Save(content, cancellationToken);

        if (string.IsNullOrEmpty(saveContentResult))
            return MakeErrorResult<string>("Error to update content");

        return new()
        {
            Success = true,
            Data = contentId
        };
    }

    public async Task<ServiceResult<string>> CopyContentsToEnrollUser(SummaryMatriculationRequest request, CancellationToken cancellationToken = default)
    {
        var summaryServiceResponse = await _summaryService.Get(request.SummaryId, cancellationToken);

        if (!summaryServiceResponse.Success)
            return MakeErrorResult<string>("Summary not found.");

        var summary = summaryServiceResponse.Data;

        if (!summary.OwnerId.Equals("iacademy"))
            return MakeErrorResult<string>("This summary is not master.");

        summary.Id = Guid.NewGuid().ToString();
        summary.OriginId = request.SummaryId;
        summary.OwnerId = request.OwnerId;

        var contentIds = summary.Topics
            .SelectMany(topic => topic.Subtopics)
            .Where(subtopic => subtopic != null && !string.IsNullOrEmpty(subtopic.ContentId))
            .Select(subtopic => subtopic.ContentId)
            .ToList();

        if (!contentIds.Any())
            return MakeErrorResult<string>("Error to get content ids.");

        var baseContents = await _contentRepository.GetAllByIds(contentIds, cancellationToken);

        if (!baseContents.Any())
            return MakeErrorResult<string>("Error to get contents.");

        var exerciseIds = baseContents
            .Where(content => !string.IsNullOrEmpty(content.ExerciseId))
            .Select(content => content.ExerciseId)
            .ToList();

        if (!exerciseIds.Any())
            return MakeErrorResult<string>("Error to get exercise ids.");

        var baseExercises = await _exerciseService.GetAllByIds(exerciseIds, cancellationToken);

        if(!baseExercises.Success)
            return MakeErrorResult<string>("Error to get exercises.");

        summary = await CreateNewReferences(baseContents, baseExercises.Data, summary);

        var newContentsSaveResult = await _contentRepository.SaveAll(baseContents, cancellationToken);

        if(!newContentsSaveResult.Any())
            return MakeErrorResult<string>("Error to save new contents.");

        var newExercisesSaveResult = await _exerciseService.SaveAll(baseExercises.Data, cancellationToken);

        if (!newExercisesSaveResult.Success)
            return MakeErrorResult<string>("Error to save new exercises.");

        var summarySaveRequest = MakeSummaryRequest(summary);

        var summaryRepositoryResponse = await _summaryService.Save(summarySaveRequest, summary.Id, cancellationToken);

        if(!summaryRepositoryResponse.Success)
            return MakeErrorResult<string>("Fail to enroll user, try again.");

        return new ServiceResult<string>()
        {
            Success = summaryRepositoryResponse.Success,
            Data = summaryRepositoryResponse.Data
        };
    }

    private static SummaryRequest MakeSummaryRequest(Summary summary) => new()
    {
        OriginId = summary.OriginId,
        OwnerId = summary.OwnerId,
        ConfigurationId = summary.ConfigurationId,
        IsAvaliable = true,
        Category = summary.Category,
        Subcategory = summary.Subcategory,
        Theme = summary.Theme,
        Topics = summary.Topics
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
                {
                    subtopic.ContentId = contentIdMap[subtopic.ContentId];
                }
            });
        });

        return summary;
    }

    private static ServiceResult<T> MakeErrorResult<T>(string message) => new()
    {
        Success = false,
        ErrorMessage = message,
        Data = default
    };

    private static List<Body> MakeNewContentList(List<Body> contents, List<Body> newContent)
    {
        var contentToDesactive = contents.Find(b => b.DisabledDate == DateTime.MinValue);

        contentToDesactive.DisabledDate = DateTime.UtcNow;

        contents.Remove(contentToDesactive);

        var newContentList = new List<Body>
            {
                contentToDesactive,
                new Body()
                {
                    Content = newContent.First().Content,
                    CreatedDate = DateTime.UtcNow
                }
            };

        newContentList.AddRange(contents.FindAll(b => b.DisabledDate != DateTime.MinValue));

        return newContentList;
    }

    private static string MakeOpenAIFirstContentRequest(InputProperties configuration, string topicIndex)
    {
        var request = $"{configuration.InitialInput}: {topicIndex} {configuration.FinalInput}";

        return request;
    }

    private static string MakeOpenAIRequest(InputProperties configuration, string theme, string contentTitle, string content)
    {
        var request = $"{configuration.InitialInput}: {theme} - {contentTitle} - {content} {configuration.FinalInput}";

        return request;
    }
}