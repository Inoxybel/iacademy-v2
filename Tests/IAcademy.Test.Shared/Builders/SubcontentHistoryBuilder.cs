using Domain.Entities.Contents;

namespace IAcademy.Test.Shared.Builders;

public class SubcontentHistoryBuilder
{
    private readonly SubcontentHistory subcontentHistory;

    public SubcontentHistoryBuilder() => subcontentHistory = CreateDefault();

    private static SubcontentHistory CreateDefault() => new SubcontentHistory
    {
        Content = string.Empty,
        CreatedDate = DateTime.UtcNow,
        DisabledDate = DateTime.MinValue
    };

    public SubcontentHistoryBuilder WithContent(string content)
    {
        subcontentHistory.Content = content;
        return this;
    }

    public SubcontentHistoryBuilder WithCreatedDate(DateTime createdDate)
    {
        subcontentHistory.CreatedDate = createdDate;
        return this;
    }

    public SubcontentHistoryBuilder WithDisabledDate(DateTime disabledDate)
    {
        subcontentHistory.DisabledDate = disabledDate;
        return this;
    }

    public SubcontentHistory Build() => subcontentHistory;
}

