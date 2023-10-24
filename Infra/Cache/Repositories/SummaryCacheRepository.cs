using CrossCutting.Constants;
using CrossCutting.Helpers;
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

    public async Task<PaginatedResult<Summary>> GetAllByCategory(PaginationRequest pagination, List<string> summaryIds, string category, bool isAvaliable = true, CancellationToken cancellationToken = default)
    {
        return await _summaryRepository.GetAllByCategory(pagination, summaryIds, category, isAvaliable, cancellationToken);
    }

    public async Task<PaginatedResult<Summary>> GetAllByCategoryAndSubcategory(PaginationRequest pagination, List<string> summaryIds, string category, string subcategory, bool isAvaliable = true, CancellationToken cancellationToken = default)
    {
        return await _summaryRepository.GetAllByCategoryAndSubcategory(pagination, summaryIds, category, subcategory, isAvaliable, cancellationToken);
    }

    public async Task<PaginatedResult<Summary>> GetAllByIds(PaginationRequest pagination, List<string> summaryIds, bool isAvaliable = true, CancellationToken cancellationToken = default)
    {
        return await _summaryRepository.GetAllByIds(pagination, summaryIds, isAvaliable, cancellationToken);
    }

    public async Task<PaginatedResult<Summary>> GetAllByOwnerId(PaginationRequest pagination, string ownerId, bool isAvaliable = true, CancellationToken cancellationToken = default)
    {
        return await _summaryRepository.GetAllByOwnerId(pagination, ownerId, isAvaliable, cancellationToken);
    }

    public async Task<PaginatedResult<Summary>> GetAllBySubcategory(PaginationRequest pagination, List<string> summaryIds, string subcategory, bool isAvaliable = true, CancellationToken cancellationToken = default)
    {
        return await _summaryRepository.GetAllBySubcategory(pagination, summaryIds, subcategory, isAvaliable, cancellationToken);
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
