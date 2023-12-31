﻿using CrossCutting.Enums;
using Domain.DTO;
using Domain.Entities;
using Domain.Infra;
using Domain.Services;
using Service.Integrations.OpenAI.Interfaces;
using CrossCutting.Extensions;
using Domain.DTO.Content;

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
            return MakeErrorResult<string>("Error while getting content");

        var configurationGetResult = await _configurationService.Get(content.ConfigurationId, cancellationToken);

        if (!configurationGetResult.Success)
            return MakeErrorResult<string>(configurationGetResult.ErrorMessage);

        return await Execute(content, configurationGetResult.Data, cancellationToken);
    }

    public async Task<ServiceResult<string>> MakeExercise(Content content, Configuration configuration, CancellationToken cancellationToken = default) =>
        await Execute(content, configuration, cancellationToken);

    private async Task<ServiceResult<string>> Execute(Content content, Configuration configuration, CancellationToken cancellationToken = default)
    {
        var openAIRequest = MakeOpenAIRequestToExercise(configuration.Exercise, content.Body.First(c => c.DisabledDate == DateTime.MinValue).Content);

        var openAIResponse = await _openAIService.DoRequest(openAIRequest);

        if (string.IsNullOrEmpty(openAIResponse.Id))
            return MakeErrorResult<string>("Error to get OpenAI response");

        var exercise = openAIResponse.Choices.First().Message.Content.Deserialize<Exercise>();
        exercise.Id = Guid.NewGuid().ToString();
        exercise.OwnerId = content.OwnerId;
        exercise.ConfigurationId = configuration.Id;
        exercise.ContentId = content.Id;
        exercise.Status = ExerciseStatus.WaitingToDo;
        exercise.Type = ExerciseType.Default;
        exercise.TopicIndex = content.SubtopicIndex;
        exercise.Title = content.Title;

        var exerciseSaveResponse = await _exerciseRepository.Save(exercise, cancellationToken);

        if (!exerciseSaveResponse)
            return MakeErrorResult<string>("Error to save exercise");

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
            return MakeErrorResult<string>("Error to update content");

        return new()
        {
            Success = true,
            Data = exercise.Id
        };
    }

    public async Task<ServiceResult<string>> MakePendency(string contentId, string oldExercise, CancellationToken cancellationToken = default)
    {
        var content = await _contentRepository.Get(contentId, cancellationToken);

        if (content is null)
            return MakeErrorResult<string>("Error while getting content");

        var configurationGetResult = await _configurationService.Get(content.ConfigurationId, cancellationToken);

        if (!configurationGetResult.Success)
            return MakeErrorResult<string>(configurationGetResult.ErrorMessage);

        var openAIRequest = MakeOpenAIRequestToPendency(
            configurationGetResult.Data.Pendency, 
            content.Body.First(c => c.DisabledDate == DateTime.MinValue).Content,
            oldExercise
        );

        var openAIResponse = await _openAIService.DoRequest(openAIRequest);

        if (string.IsNullOrEmpty(openAIResponse.Id))
            return MakeErrorResult<string>("Error to get OpenAI response");

        var exercise = openAIResponse.Choices.First().Message.Content.Deserialize<Exercise>();
        exercise.Id = Guid.NewGuid().ToString();
        exercise.OwnerId = content.OwnerId;
        exercise.ConfigurationId = configurationGetResult.Data.Id;
        exercise.ContentId = content.Id;
        exercise.Status = ExerciseStatus.WaitingToDo;
        exercise.Type = ExerciseType.Pendency;
        exercise.TopicIndex = content.SubtopicIndex;
        exercise.Title = content.Title;

        var exerciseSaveResponse = await _exerciseRepository.Save(exercise, cancellationToken);

        if (!exerciseSaveResponse)
            return MakeErrorResult<string>("Error to save exercise");

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
            return MakeErrorResult<string>("Error to update content");

        return new()
        {
            Success = true,
            Data = exercise.Id
        };
    }

    private static ServiceResult<T> MakeErrorResult<T>(string message) => new()
    {
        Success = false,
        ErrorMessage = message,
        Data = default
    };

    private static string MakeOpenAIRequestToExercise(InputProperties configuration, string content) =>
        $"{configuration.InitialInput} {content} {configuration.FinalInput}";

    private static string MakeOpenAIRequestToPendency(InputProperties configuration, string content, string oldExercise) =>
        $"{configuration.InitialInput} {content}. Exercicio anteriormente aplicado: {oldExercise} {configuration.FinalInput}";
}
