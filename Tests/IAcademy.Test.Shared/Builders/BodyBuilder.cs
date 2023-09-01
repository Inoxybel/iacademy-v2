using Domain.Entities;

namespace IAcademy.Test.Shared.Builders;

public class BodyBuilder
{
    private Body body;

    public BodyBuilder() => body = CreateDefault();

    private static Body CreateDefault() => new Body
    {
        Content = "DefaultContent",
        CreatedDate = DateTime.UtcNow,
        DisabledDate = DateTime.MinValue
    };

    public BodyBuilder WithContent(string content)
    {
        body.Content = content;
        return this;
    }

    public BodyBuilder WithCreatedDate(DateTime createdDate)
    {
        body.CreatedDate = createdDate;
        return this;
    }

    public BodyBuilder WithDisabledDate(DateTime disabledDate)
    {
        body.DisabledDate = disabledDate;
        return this;
    }

    public Body Build() => body;
}