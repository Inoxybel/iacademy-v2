using Domain.DTO.Summary;
using Domain.Entities;

namespace Domain.Infra;

public interface ISummaryRepository
{
    Task<Summary> Get(string id, CancellationToken cancellationToken = default);
    Task<List<Summary>> GetAllByOwnerId(string ownerId, bool isAvaliable = false, CancellationToken cancellationToken = default);
    Task<List<Summary>> GetAllByCategory(string category, bool isAvaliable = false, CancellationToken cancellationToken = default);
    Task<List<Summary>> GetAllBySubcategory(string subcategory, bool isAvaliable = false, CancellationToken cancellationToken = default);
    Task<List<Summary>> GetAllByCategoryAndSubcategory(string category, string subcategory, bool isAvaliable = false, CancellationToken cancellationToken = default);
    Task<bool> Save(Summary summary, CancellationToken cancellationToken = default);
    Task<bool> Update(string summaryId, SummaryRequest summary, CancellationToken cancellationToken = default);
}