using Domain.Entities;

namespace IAcademy.Test.Shared.Builders;

public class SubtopicBuilder
{
    private Subtopic subtopic;

    public SubtopicBuilder() => subtopic = CreateDefault();

    private static Subtopic CreateDefault() => new()
    {
        Index = "1.1",
        Title = "Title",
        ContentId = Guid.NewGuid().ToString()
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

    public Subtopic Build() => subtopic;
}