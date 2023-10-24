using Domain.Entities.Contents;

namespace IAcademy.Test.Shared.Builders;

public class ContentBuilder
{
    private Content content;

    public ContentBuilder() => content = CreateDefault();

    private static Content CreateDefault() => new()
    {
        Id = Guid.NewGuid().ToString(),
        OwnerId = "DefaultOwnerId",
        SummaryId = "DefaultSummaryId",
        ConfigurationId = "DefaultConfigurationId",
        ExerciseId = "DefaultExerciseId",
        Theme = "DefaultTheme",
        SubtopicIndex = "1",
        Title = "DefaultTitle",
        Body = new BodyBuilder().Build(),
        CreatedDate = DateTime.UtcNow,
        UpdatedDate = DateTime.UtcNow
    };

    public ContentBuilder WithId(string id)
    {
        content.Id = id;
        return this;
    }

    public ContentBuilder WithOriginId(string originId)
    {
        content.OriginId = originId;
        return this;
    }

    public ContentBuilder WithOwnerId(string ownerId)
    {
        content.OwnerId = ownerId;
        return this;
    }

    public ContentBuilder WithSummaryId(string summaryId)
    {
        content.SummaryId = summaryId;
        return this;
    }

    public ContentBuilder WithConfigurationId(string configurationId)
    {
        content.ConfigurationId = configurationId;
        return this;
    }

    public ContentBuilder WithExerciseId(string ExerciseId)
    {
        content.ExerciseId = ExerciseId;
        return this;
    }

    public ContentBuilder WithTheme(string theme)
    {
        content.Theme = theme;
        return this;
    }

    public ContentBuilder WithSubtopicIndex(string subtopicIndex)
    {
        content.SubtopicIndex = subtopicIndex;
        return this;
    }

    public ContentBuilder WithTitle(string title)
    {
        content.Title = title;
        return this;
    }

    public ContentBuilder WithBody(Body body)
    {
        content.Body = body;
        return this;
    }

    public ContentBuilder WithCreatedDate(DateTime createdDate)
    {
        content.CreatedDate = createdDate;
        return this;
    }

    public ContentBuilder WithUpdatedDate(DateTime updatedDate)
    {
        content.UpdatedDate = updatedDate;
        return this;
    }

    public Content Build() => content;

}