using Domain.Entities;

namespace Domain.Infra;

public interface ICorrectionRepository
{
    Task<Correction> Get(string correctionId, CancellationToken cancellationToken = default);
    Task<bool> Save(Correction correction, CancellationToken cancellationToken = default);
    Task<bool> Update(string correctionId, Correction correction, CancellationToken cancellationToken = default);
}