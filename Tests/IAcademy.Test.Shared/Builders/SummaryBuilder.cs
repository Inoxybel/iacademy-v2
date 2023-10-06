using Domain.Entities;
using Domain.Entities.Summary;

namespace IAcademy.Test.Shared.Builders;

public class SummaryBuilder
{
    private Summary summary;

    public SummaryBuilder() => summary = CreateDefault();

    private static Summary CreateDefault() => new()
    {
        Id = Guid.NewGuid().ToString(),
        OriginId = Guid.NewGuid().ToString(),
        OwnerId = Guid.NewGuid().ToString(),
        ConfigurationId = Guid.NewGuid().ToString(),
        CreatedDate = DateTime.UtcNow,
        UpdatedDate = DateTime.UtcNow,
        IsAvaliable = false,
        Category = "Category",
        Subcategory = "Subcategory",
        Theme = "Theme",
        Icon = "Icon",
        Topics = new()
        {
            new TopicBuilder().Build()
        }
    };

    public SummaryBuilder WithId(string id)
    {
        summary.Id = id;
        return this;
    }

    public SummaryBuilder WithOriginId(string originId)
    {
        summary.OriginId = originId;
        return this;
    }

    public SummaryBuilder WithOwnerId(string ownerId)
    {
        summary.OwnerId = ownerId;
        return this;
    }

    public SummaryBuilder WithConfigurationId(string configurationId)
    {
        summary.ConfigurationId = configurationId;
        return this;
    }

    public SummaryBuilder WithCreatedDate(DateTime createdDate)
    {
        summary.CreatedDate = createdDate;
        return this;
    }

    public SummaryBuilder WithUpdatedDate(DateTime updatedDate)
    {
        summary.UpdatedDate = updatedDate;
        return this;
    }

    public SummaryBuilder WithIsAvaliable(bool isAvaliable)
    {
        summary.IsAvaliable = isAvaliable;
        return this;
    }

    public SummaryBuilder WithCategory(string category)
    {
        summary.Category = category;
        return this;
    }

    public SummaryBuilder WithSubcategory(string subcategory)
    {
        summary.Subcategory = subcategory;
        return this;
    }

    public SummaryBuilder WithTheme(string theme)
    {
        summary.Theme = theme;
        return this;
    }

    public SummaryBuilder WithTopics(List<Topic> topics)
    {
        summary.Topics = topics;
        return this;
    }

    public Summary Build() => summary;
}