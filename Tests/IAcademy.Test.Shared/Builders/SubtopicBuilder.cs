using Domain.Entities.Summary;

namespace IAcademy.Test.Shared.Builders;

public class SubtopicBuilder
{
    private Subtopic subtopic;

    public SubtopicBuilder() => subtopic = CreateDefault();

    private static Subtopic CreateDefault() => new()
    {
        Index = "1.1",
        Title = "Title",
        ContentId = "DefaultContentId",
        ExerciseId = "DefaultExerciseId"
    };

    public SubtopicBuilder WithIndex(string index)
    {
        subtopic.Index = index;
        return this;
    }

    public SubtopicBuilder WithTitle(string title)
    {
        subtopic.Title = title;
        return this;
    }

    public SubtopicBuilder WithContentId(string contentId)
    {
        subtopic.ContentId = contentId;
        return this;
    }

    public SubtopicBuilder WithExerciseId(string exerciseId)
    {
        subtopic.ExerciseId = exerciseId;
        return this;
    }

    public Subtopic Build() => subtopic;
}