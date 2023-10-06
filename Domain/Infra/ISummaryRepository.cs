using Domain.DTO.Summary;
using Domain.Entities.Summary;

namespace Domain.Infra;

public interface ISummaryRepository
{
    Task<Summary> Get(string summaryId, CancellationToken cancellationToken = default);
    Task<bool> IsEnrolled(string summaryId, string ownerId, CancellationToken cancellationToken = default);
    Task<bool> ShouldGeneratePendency(string summaryId, string ownerId, CancellationToken cancellationToken = default);
    Task<List<string>> IsEnrolled(List<string> summaryIds, string ownerId, CancellationToken cancellationToken = default);
    Task<List<Summary>> GetAllByIds(List<string> summaryIds, bool isAvaliable = true, CancellationToken cancellationToken = default);
    Task<List<Summary>> GetAllByOwnerId(string ownerId, bool isAvaliable = true, CancellationToken cancellationToken = default);
    Task<List<Summary>> GetAllByCategory(List<string> summaryIds, string category, bool isAvaliable = true, CancellationToken cancellationToken = default);
    Task<List<Summary>> GetAllBySubcategory(List<string> summaryIds, string subcategory, bool isAvaliable = true, CancellationToken cancellationToken = default);
    Task<List<Summary>> GetAllByCategoryAndSubcategory(List<string> summaryIds, string category, string subcategory, bool isAvaliable = true, CancellationToken cancellationToken = default);
    Task<bool> Save(Summary summary, CancellationToken cancellationToken = default);
    Task<bool> Update(string summaryId, SummaryRequest summary, CancellationToken cancellationToken = default);
}