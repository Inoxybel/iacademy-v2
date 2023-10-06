using Domain.DTO.Correction;
using Domain.DTO;
using Domain.Entities.Feedback;

namespace Domain.Services;

public interface ICorrectionService
{
    Task<ServiceResult<Correction>> Get(string correctionId, string ownerId, CancellationToken cancellationToken = default);
    Task<ServiceResult<Correction>> MakeCorrection(string exerciseId, string ownerId, CreateCorrectionRequest request, CancellationToken cancellationToken = default);
    Task<ServiceResult<bool>> Update(string correctionId, CorrectionUpdateRequest request, CancellationToken cancellationToken = default);
}