using Domain.DTO.Summary;
using Domain.DTO;
using Domain.Entities;

namespace Domain.Services;

public interface ISummaryService
{
    Task<ServiceResult<Summary>> Get(string summaryId, CancellationToken cancellationToken = default);
    Task<ServiceResult<List<Summary>>> GetAllByOwnerId(string ownerId, bool isAvaliable = false, CancellationToken cancellationToken = default);
    Task<ServiceResult<List<Summary>>> GetAllByCategory(string category, bool isAvaliable = false, CancellationToken cancellationToken = default);
    Task<ServiceResult<List<Summary>>> GetAllBySubcategory(string subcategory, bool isAvaliable = false, CancellationToken cancellationToken = default);
    Task<ServiceResult<List<Summary>>> GetAllByCategoryAndSubcategory(string category, string subcategory, bool isAvaliable = false, CancellationToken cancellationToken = default);
    Task<ServiceResult<string>> Save(SummaryRequest request, string newId = "", CancellationToken cancellationToken = default);
    Task<ServiceResult<string>> Update(string summaryId, SummaryRequest request, CancellationToken cancellationToken = default);
    Task<ServiceResult<string>> RequestCreationToAI(SummaryCreationRequest request, CancellationToken cancellationToken = default);
}