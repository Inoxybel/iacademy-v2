using Domain.Entities.Summary;

namespace IAcademy.Test.Shared.Builders;

public class TopicBuilder
{
    private Topic topic;

    public TopicBuilder() => topic = CreateDefault();

    private static Topic CreateDefault() => new()
    {
        Index = "1",
        Title = "Title",
        Description = "Description",
        Subtopics = new()
        {
            new SubtopicBuilder().Build()
        }
    };

    public TopicBuilder WithIndex(string index)
    {
        topic.Index = index;
        return this;
    }

    public TopicBuilder WithTitle(string title)
    {
        topic.Title = title;
        return this;
    }

    public TopicBuilder WithDescription(string description)
    {
        topic.Description = description;
        return this;
    }

    public TopicBuilder WithSubtopics(List<Subtopic> subtopics)
    {
        topic.Subtopics = subtopics;
        return this;
    }

    public Topic Build() => topic;
}