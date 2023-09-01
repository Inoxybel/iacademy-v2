using CrossCutting.Enums;
using Domain.Entities;

namespace IAcademy.Test.Shared.Builders;

public class ExerciseBuilder
{
    private Exercise exercise;

    public ExerciseBuilder() => exercise = CreateDefault();

    private static Exercise CreateDefault() => new()
    {
        Id = Guid.NewGuid().ToString(),
        OwnerId = string.Empty,
        CorrectionId = string.Empty,
        ConfigurationId = string.Empty,
        Status = ExerciseStatus.WaitingToDo,
        Type = ExerciseType.Default, 
        SendedAt = DateTime.UtcNow,
        TopicIndex = "1",
        Title = "DefaultTitle",
        Exercises = new List<Activity>()
        {
            new ActivityBuilder().Build()
        }
    };

    public ExerciseBuilder WithId(string id)
    {
        exercise.Id = id;
        return this;
    }

    public ExerciseBuilder WithOwnerId(string ownerId)
    {
        exercise.OwnerId = ownerId;
        return this;
    }

    public ExerciseBuilder WithCorrectionId(string correctionId)
    {
        exercise.CorrectionId = correctionId;
        return this;
    }

    public ExerciseBuilder WithConfigurationId(string configurationId)
    {
        exercise.ConfigurationId = configurationId;
        return this;
    }

    public ExerciseBuilder WithStatus(ExerciseStatus status)
    {
        exercise.Status = status;
        return this;
    }

    public ExerciseBuilder WithType(ExerciseType type)
    {
        exercise.Type = type;
        return this;
    }

    public ExerciseBuilder WithSendedAt(DateTime sendedAt)
    {
        exercise.SendedAt = sendedAt;
        return this;
    }

    public ExerciseBuilder WithTopicIndex(string topicIndex)
    {
        exercise.TopicIndex = topicIndex;
        return this;
    }

    public ExerciseBuilder WithTitle(string title)
    {
        exercise.Title = title;
        return this;
    }

    public ExerciseBuilder WithExercises(List<Activity> exercises)
    {
        exercise.Exercises = exercises;
        return this;
    }

    public Exercise Build() => exercise;
}