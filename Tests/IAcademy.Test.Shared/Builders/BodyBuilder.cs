using Domain.Entities.Contents;

namespace IAcademy.Test.Shared.Builders;

public class BodyBuilder
{
    private readonly Body body;

    public BodyBuilder() => body = CreateDefault();

    private static Body CreateDefault() => new Body
    {
        Contents = new()
        {
            new SubcontentBuilder().Build()
        }
    };

    public BodyBuilder WithContents(List<Subcontent> subcontents)
    {
        body.Contents = subcontents;
        return this;
    }

    public Body Build() => body;
}