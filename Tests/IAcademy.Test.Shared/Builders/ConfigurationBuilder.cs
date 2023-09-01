using Domain.Entities;

namespace IAcademy.Test.Shared.Builders;

public class ConfigurationBuilder
{
    private Configuration configuration;

    public ConfigurationBuilder() => configuration = CreateDefault();

    private static Configuration CreateDefault() => new()
    {
        Id = Guid.NewGuid().ToString(),
        Summary = new InputPropertiesBuilder().Build(),
        FirstContent = new InputPropertiesBuilder().Build(),
        NewContent = new InputPropertiesBuilder().Build(),
        Exercise = new InputPropertiesBuilder().Build(),
        Correction = new InputPropertiesBuilder().Build(),
        Pendency = new InputPropertiesBuilder().Build(),
    };

    public ConfigurationBuilder WithId(string id)
    {
        configuration.Id = id;
        return this;
    }

    public ConfigurationBuilder WithSummary(InputProperties summary)
    {
        configuration.Summary = summary;
        return this;
    }

    public ConfigurationBuilder WithFirstContent(InputProperties firstContent)
    {
        configuration.FirstContent = firstContent;
        return this;
    }

    public ConfigurationBuilder WithNewContent(InputProperties newContent)
    {
        configuration.NewContent = newContent;
        return this;
    }

    public ConfigurationBuilder WithExercise(InputProperties exercise)
    {
        configuration.Exercise = exercise;
        return this;
    }

    public ConfigurationBuilder WithCorrection(InputProperties correction)
    {
        configuration.Correction = correction;
        return this;
    }

    public ConfigurationBuilder WithPendency(InputProperties pendency)
    {
        configuration.Pendency = pendency;
        return this;
    }

    public Configuration Build() => configuration;
}