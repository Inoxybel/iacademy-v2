using CrossCutting.Constants;
using Domain.Entities.Feedback;
using Domain.Infra;
using Infra.Cache.Interfaces;
using Infra.Cache.Options;
using Microsoft.Extensions.Options;

namespace Infra.Cache.Repositories;

public class CorrectionCacheRepository : BaseRedisCacheRepository, ICorrectionRepository
{
    private readonly ICorrectionRepository _correctionRepository;

    public CorrectionCacheRepository(
        ICorrectionRepository correctionRepository,
        IRedisConnectionManager connectionManager,
        IOptionsSnapshot<CacheOptions> cacheOptions)
        : base(
            connectionManager,
            cacheOptions.Value,
            AppConstants.AppName,
            nameof(Correction))
    {
        _correctionRepository = correctionRepository;
    }

    public async Task<Correction> Get(string correctionId, CancellationToken cancellationToken = default)
    {
        var correction = await GetOrRefreshCache<Correction>(correctionId,
            () => _correctionRepository.Get(correctionId, cancellationToken)
        );

        return correction;
    }

    public async Task<bool> Save(Correction correction, CancellationToken cancellationToken = default)
    {
        return await _correctionRepository.Save(correction, cancellationToken);
    }

    public async Task<bool> Update(string correctionId, Correction correction, CancellationToken cancellationToken = default)
    {
        return await _correctionRepository.Update(correctionId, correction, cancellationToken);
    }
}
