using CrossCutting.Enums;
using Domain.DTO;
using Domain.Infra;
using Domain.Services;
using Service.Integrations.OpenAI.Interfaces;
using CrossCutting.Extensions;
using Domain.DTO.Content;
using Domain.Entities.Exercise;
using Domain.Entities.Configuration;
using Domain.Entities.Contents;

namespace Service;

public class GeneratorService : IGeneratorService
{
    private readonly IConfigurationService _configurationService;
    private readonly IExerciseRepository _exerciseRepository;
    private readonly IContentRepository _contentRepository;
    private readonly IOpenAIService _openAIService;

    public GeneratorService(
        IConfigurationService configurationService,
        IExerciseRepository exerciseRepository,
        IContentRepository contentRepository,
        IOpenAIService openAIService)
    {
        _configurationService = configurationService;
        _exerciseRepository = exerciseRepository;
        _contentRepository = contentRepository;
        _openAIService = openAIService;
    }

    public async Task<ServiceResult<string>> MakeExercise(string contentId, CancellationToken cancellationToken = default)
    {
        var content = await _contentRepository.Get(contentId, cancellationToken);

        if (content is null)
            return ServiceResult<string>.MakeErrorResult("Error while getting content");

        var configurationGetResult = await _configurationService.Get(content.ConfigurationId, cancellationToken);

        if (!configurationGetResult.Success)
            return ServiceResult<string>.MakeErrorResult(configurationGetResult.ErrorMessage);

        return await Execute(content, configurationGetResult.Data.Exercise, configurationGetResult.Data.Id, cancellationToken);
    }

    public async Task<ServiceResult<string>> MakeExercise(Content content, InputProperties configuration, string configurationId, CancellationToken cancellationToken = default) =>
        await Execute(content, configuration, configurationId, cancellationToken);

    public async Task<ServiceResult<string>> MakePendency(string contentId, string correction, Configuration configuration, CancellationToken cancellationToken = default)
    {
        var content = await _contentRepository.Get(contentId, cancellationToken);

        if (content is null)
            return ServiceResult<string>.MakeErrorResult("Error while getting content");

        var openAIResponse = await _openAIService.DoRequest(configuration.Pendency, correction);

        if (string.IsNullOrEmpty(openAIResponse.Id))
            return ServiceResult<string>.MakeErrorResult("Error to get OpenAI response");

        var exercise = openAIResponse.Choices.First().Message.Content.Deserialize<Exercise>();
        exercise.Id = Guid.NewGuid().ToString();
        exercise.OwnerId = content.OwnerId;
        exercise.ConfigurationId = configuration.Id;
        exercise.ContentId = content.Id;
        exercise.Status = ExerciseStatus.WaitingToDo;
        exercise.Type = ExerciseType.Pendency;
        exercise.TopicIndex = content.SubtopicIndex;
        exercise.Title = content.Title;

        var exerciseSaveResponse = await _exerciseRepository.Save(exercise, cancellationToken);

        if (!exerciseSaveResponse)
            return ServiceResult<string>.MakeErrorResult("Error to save exercise");

        content.ExerciseId = exercise.Id;

        var contentRequest = new ContentRequest()
        {
            OwnerId = content.OwnerId,
            SummaryId = content.SummaryId,
            ConfigurationId = content.ConfigurationId,
            ExerciseId = exercise.Id,
            Theme = content.Theme,
            SubtopicIndex = content.SubtopicIndex,
            Title = content.Title,
            Body = content.Body
        };

        var contentSaveResponse = await _contentRepository.Update(content.Id, contentRequest, cancellationToken);

        if (!contentSaveResponse)
            return ServiceResult<string>.MakeErrorResult("Error to update content");

        return ServiceResult<string>.MakeSuccessResult(exercise.Id);
    }

    private async Task<ServiceResult<string>> Execute(Content content, InputProperties configuration, string configurationId, CancellationToken cancellationToken = default)
    {
        var openAIResponse = await _openAIService.DoRequest(configuration, content.Body.GetAllActiveContent());

        if (string.IsNullOrEmpty(openAIResponse.Id))
            return ServiceResult<string>.MakeErrorResult("Error to get OpenAI response");

        var exercise = openAIResponse.Choices.First().Message.Content.Deserialize<Exercise>();
        exercise.Id = Guid.NewGuid().ToString();
        exercise.OwnerId = content.OwnerId;
        exercise.ConfigurationId = configurationId;
        exercise.ContentId = content.Id;
        exercise.Status = ExerciseStatus.WaitingToDo;
        exercise.Type = ExerciseType.Default;
        exercise.TopicIndex = content.SubtopicIndex;
        exercise.Title = content.Title;
        exercise.SummaryId = content.SummaryId;

        var exerciseSaveResponse = await _exerciseRepository.Save(exercise, cancellationToken);

        if (!exerciseSaveResponse)
            return ServiceResult<string>.MakeErrorResult("Error to save exercise");

        content.ExerciseId = exercise.Id;

        var contentRequest = new ContentRequest()
        {
            OwnerId = content.OwnerId,
            SummaryId = content.SummaryId,
            ConfigurationId = content.ConfigurationId,
            ExerciseId = exercise.Id,
            Theme = content.Theme,
            SubtopicIndex = content.SubtopicIndex,
            Title = content.Title,
            Body = content.Body
        };

        var contentSaveResponse = await _contentRepository.Update(content.Id, contentRequest, cancellationToken);

        if (!contentSaveResponse)
            return ServiceResult<string>.MakeErrorResult("Error to update content");

        return ServiceResult<string>.MakeSuccessResult(exercise.Id);
    }
}
