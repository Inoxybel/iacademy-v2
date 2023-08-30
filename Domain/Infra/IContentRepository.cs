namespace Domain.Infra;

public interface IContentRepository
{
    Task<Content> Get(string id, CancellationToken cancellationToken = default);
    Task<List<Content>> GetAllBySummaryId(string summaryId, CancellationToken cancellationToken = default);
    Task<string> Save(Content content, CancellationToken cancellationToken = default);
    Task<List<string>> SaveAll(List<Content> contents, CancellationToken cancellationToken = default);
    Task<bool> Update(string contentId, ContentRequest request, CancellationToken cancellationToken = default);
    Task<bool> UpdateAll(string summaryId, List<Content> contents, CancellationToken cancellationToken = default);
}