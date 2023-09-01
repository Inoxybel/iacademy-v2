using Domain.DTO.Content;
using Domain.Entities;

namespace IAcademy.Test.Shared.Builders;

public class ContentRequestBuilder
{
    private ContentRequest contentRequest;

    public ContentRequestBuilder() => contentRequest = CreateDefault();

    private static ContentRequest CreateDefault() => new ContentRequest
    {
        OwnerId = string.Empty,
        SummaryId = string.Empty,
        ConfigurationId = string.Empty,
        ExerciceId = string.Empty,
        Theme = "DefaultTheme",
        SubtopicIndex = "1",
        Title = "DefaultTitle",
        Body = new List<Body>()
    };

    public ContentRequestBuilder WithOwnerId(string ownerId)
    {
        contentRequest.OwnerId = ownerId;
        return this;
    }

    public ContentRequestBuilder WithSummaryId(string summaryId)
    {
        contentRequest.SummaryId = summaryId;
        return this;
    }

    public ContentRequestBuilder WithConfigurationId(string configurationId)
    {
        contentRequest.ConfigurationId = configurationId;
        return this;
    }

    public ContentRequestBuilder WithExerciceId(string exerciceId)
    {
        contentRequest.ExerciceId = exerciceId;
        return this;
    }

    public ContentRequestBuilder WithTheme(string theme)
    {
        contentRequest.Theme = theme;
        return this;
    }

    public ContentRequestBuilder WithSubtopicIndex(string subtopicIndex)
    {
        contentRequest.SubtopicIndex = subtopicIndex;
        return this;
    }

    public ContentRequestBuilder WithTitle(string title)
    {
        contentRequest.Title = title;
        return this;
    }

    public ContentRequestBuilder WithBody(List<Body> body)
    {
        contentRequest.Body = body;
        return this;
    }

    public ContentRequest Build() => contentRequest;
}