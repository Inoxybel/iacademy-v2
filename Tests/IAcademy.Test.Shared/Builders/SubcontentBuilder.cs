using Domain.Entities.Contents;

namespace IAcademy.Test.Shared.Builders;

public class SubcontentBuilder
{
    private readonly Subcontent subcontent;

    public SubcontentBuilder() => subcontent = CreateDefault();

    private static Subcontent CreateDefault() => new Subcontent
    {
        SubcontentHistory = new()
        {
            new SubcontentHistoryBuilder().Build()
        }
    };

    public SubcontentBuilder WithSubcontentHistory(List<SubcontentHistory> subcontentsHistory)
    {
        subcontent.SubcontentHistory = subcontentsHistory;
        return this;
    }

    public Subcontent Build() => subcontent;
}

