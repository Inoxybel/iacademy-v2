using Domain.DTO.Summary;
using Domain.DTO;
using Domain.Entities.Summary;

namespace Domain.Services;

public interface ISummaryService
{
    Task<ServiceResult<Summary>> Get(string summaryId, CancellationToken cancellationToken = default);
    Task<bool> IsEnrolled(string summaryId, string ownerId, CancellationToken cancellationToken = default);
    Task<bool> ShouldGeneratePendency(string summaryId, string ownerId, CancellationToken cancellationToken = default);
    Task<List<string>> IsEnrolled(List<string> summaryIds, string ownerId, CancellationToken cancellationToken = default);
    Task<ServiceResult<List<Summary>>> GetAllByOwnerId(string ownerId, bool isAvaliable = false, CancellationToken cancellationToken = default);
    Task<ServiceResult<List<Summary>>> GetAllAvaliableByDocument(string ownerId, string document, string companyRef, bool isAvaliable = false, CancellationToken cancellationToken = default);
    Task<ServiceResult<List<Summary>>> GetAllByCategory(string category, string document, string companyRef, bool isAvaliable = false, CancellationToken cancellationToken = default);
    Task<ServiceResult<List<Summary>>> GetAllBySubcategory(string subcategory, string document, string companyRef, bool isAvaliable = false, CancellationToken cancellationToken = default);
    Task<ServiceResult<List<Summary>>> GetAllByCategoryAndSubcategory(string category, string subcategory, string document, string companyRef, bool isAvaliable = false, CancellationToken cancellationToken = default);
    Task<ServiceResult<string>> Save(SummaryRequest request, string newId = "", CancellationToken cancellationToken = default);
    Task<ServiceResult<string>> Update(string summaryId, SummaryRequest request, CancellationToken cancellationToken = default);
    Task<ServiceResult<bool>> UpdateProgress(string summaryId, string subtopicIndex, CancellationToken cancellationToken);
    Task<ServiceResult<string>> RequestCreationToAI(SummaryCreationRequest request, CancellationToken cancellationToken = default);
}