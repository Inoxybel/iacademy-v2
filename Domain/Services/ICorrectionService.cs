using Domain.DTO.Correction;
using Domain.DTO;
using Domain.Entities;

namespace Domain.Services;

public interface ICorrectionService
{
    Task<ServiceResult<Correction>> Get(string correctionId, CancellationToken cancellationToken = default);
    Task<ServiceResult<Correction>> MakeCorrection(string exerciseId, CreateCorrectionRequest request, CancellationToken cancellationToken = default);
    Task<ServiceResult<bool>> Update(string correctionId, CorrectionUpdateRequest request, CancellationToken cancellationToken = default);
}