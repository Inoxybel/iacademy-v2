using Domain.DTO.Summary;
using Domain.DTO;
using Domain.Entities.Summary;
using CrossCutting.Helpers;

namespace Domain.Services;

public interface ISummaryService
{
    Task<ServiceResult<Summary>> Get(string summaryId, CancellationToken cancellationToken = default);
    Task<bool> IsEnrolled(string summaryId, string ownerId, CancellationToken cancellationToken = default);
    Task<bool> ShouldGeneratePendency(string summaryId, string ownerId, CancellationToken cancellationToken = default);
    Task<List<string>> IsEnrolled(List<string> summaryIds, string ownerId, CancellationToken cancellationToken = default);
    Task<ServiceResult<PaginatedResult<Summary>>> GetAllByOwnerId(PaginationRequest pagination, string ownerId, bool isAvaliable = false, CancellationToken cancellationToken = default);
    Task<ServiceResult<PaginatedResult<Summary>>> GetAllAvaliableByDocument(PaginationRequest pagination, string ownerId, string document, string companyRef, bool isAvaliable = false, CancellationToken cancellationToken = default);
    Task<ServiceResult<PaginatedResult<Summary>>> GetAllByCategory(PaginationRequest pagination, string category, string document, string companyRef, bool isAvaliable = false, CancellationToken cancellationToken = default);
    Task<ServiceResult<PaginatedResult<Summary>>> GetAllBySubcategory(PaginationRequest pagination, string subcategory, string document, string companyRef, bool isAvaliable = false, CancellationToken cancellationToken = default);
    Task<ServiceResult<PaginatedResult<Summary>>> GetAllByCategoryAndSubcategory(PaginationRequest pagination, string category, string subcategory, string document, string companyRef, bool isAvaliable = false, CancellationToken cancellationToken = default);
    Task<ServiceResult<string>> Save(SummaryRequest request, string newId = "", CancellationToken cancellationToken = default);
    Task<ServiceResult<string>> Update(string summaryId, SummaryRequest request, CancellationToken cancellationToken = default);
    Task<ServiceResult<bool>> UpdateProgress(string summaryId, string subtopicIndex, CancellationToken cancellationToken);
    Task<ServiceResult<string>> RequestCreationToAI(SummaryCreationRequest request, CancellationToken cancellationToken = default);
}