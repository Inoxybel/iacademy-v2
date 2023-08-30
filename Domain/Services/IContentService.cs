using Domain.DTO.Content;
using Domain.DTO;
using Domain.Entities;

namespace Domain.Services;

public interface IContentService
{
    Task<ServiceResult<string>> MakeAlternativeContent(string contentId, CancellationToken cancellationToken = default);
    Task<ServiceResult<string>> MakeContent(string summaryId, AIContentCreationRequest request, CancellationToken cancellationToken = default);
    Task<ServiceResult<Content>> Get(string id, CancellationToken cancellationToken = default);
    Task<ServiceResult<List<Content>>> GetAllBySummaryId(string summaryId, CancellationToken cancellationToken = default);
    Task<ServiceResult<string>> Save(ContentRequest request, CancellationToken cancellationToken = default);
    Task<ServiceResult<List<string>>> SaveAll(List<ContentRequest> contents, CancellationToken cancellationToken = default);
    Task<ServiceResult<bool>> Update(string contentId, ContentRequest request, CancellationToken cancellationToken = default);
    Task<ServiceResult<bool>> UpdateAll(string summaryId, List<Content> contents, CancellationToken cancellationToken = default);
}