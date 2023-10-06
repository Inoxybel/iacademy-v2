using CrossCutting.Constants;
using Domain.DTO.Summary;
using Domain.Entities.Summary;
using Domain.Infra;
using Infra.Cache.Interfaces;
using Infra.Cache.Options;
using Microsoft.Extensions.Options;

namespace Infra.Cache.Repositories;

public class SummaryCacheRepository : BaseRedisCacheRepository, ISummaryRepository
{
    private readonly ISummaryRepository _summaryRepository;

    public SummaryCacheRepository(
        ISummaryRepository summaryRepository,
        IRedisConnectionManager redisConnectionManager,
        IOptionsSnapshot<CacheOptions> cacheOptions)
        :base(
                redisConnectionManager,
                cacheOptions.Value,
                AppConstants.AppName,
                nameof(Summary))
    {
        _summaryRepository = summaryRepository;
    }

    public async Task<Summary> Get(string summaryId, CancellationToken cancellationToken = default)
    {
        var summary = await GetOrRefreshCache<Summary>(summaryId,
           () => _summaryRepository.Get(summaryId, cancellationToken)
        );

        return summary;
    }

    public async Task<List<Summary>> GetAllByCategory(List<string> summaryIds, string category, bool isAvaliable = true, CancellationToken cancellationToken = default)
    {
        return await _summaryRepository.GetAllByCategory(summaryIds, category, isAvaliable, cancellationToken);
    }

    public async Task<List<Summary>> GetAllByCategoryAndSubcategory(List<string> summaryIds, string category, string subcategory, bool isAvaliable = true, CancellationToken cancellationToken = default)
    {
        return await _summaryRepository.GetAllByCategoryAndSubcategory(summaryIds, category, subcategory, isAvaliable, cancellationToken);
    }

    public async Task<List<Summary>> GetAllByIds(List<string> summaryIds, bool isAvaliable = true, CancellationToken cancellationToken = default)
    {
        return await _summaryRepository.GetAllByIds(summaryIds, isAvaliable, cancellationToken);
    }

    public async Task<List<Summary>> GetAllByOwnerId(string ownerId, bool isAvaliable = true, CancellationToken cancellationToken = default)
    {
        return await _summaryRepository.GetAllByOwnerId(ownerId, isAvaliable, cancellationToken);
    }

    public async Task<List<Summary>> GetAllBySubcategory(List<string> summaryIds, string subcategory, bool isAvaliable = true, CancellationToken cancellationToken = default)
    {
        return await _summaryRepository.GetAllBySubcategory(summaryIds, subcategory, isAvaliable, cancellationToken);
    }

    public async Task<bool> IsEnrolled(string summaryId, string ownerId, CancellationToken cancellationToken = default)
    {
        var isEnrolled = await GetOrRefreshCache<bool>(summaryId,
           () => _summaryRepository.IsEnrolled(summaryId, ownerId, cancellationToken),
           ownerId
        );

        return isEnrolled;
    }

    public async Task<List<string>> IsEnrolled(List<string> summaryIds, string ownerId, CancellationToken cancellationToken = default)
    {
         return await _summaryRepository.IsEnrolled(summaryIds, ownerId, cancellationToken);
    }

    public async Task<bool> Save(Summary summary, CancellationToken cancellationToken = default)
    {
        return await _summaryRepository.Save(summary, cancellationToken);
    }

    public async Task<bool> ShouldGeneratePendency(string summaryId, string ownerId, CancellationToken cancellationToken = default)
    {
        var shouldGeneratePendency = await GetOrRefreshCache<bool>(summaryId,
            () => _summaryRepository.ShouldGeneratePendency(summaryId, ownerId, cancellationToken),
            ownerId
        );

        return shouldGeneratePendency;
    }

    public async Task<bool> Update(string summaryId, SummaryRequest summary, CancellationToken cancellationToken = default)
    {
        return await _summaryRepository.Update(summaryId, summary, cancellationToken);
    }
}
