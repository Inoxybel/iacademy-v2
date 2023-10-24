using CrossCutting.Constants;
using CrossCutting.Helpers;
using Domain.DTO.Configuration;
using Domain.Entities.Configuration;
using Domain.Infra;
using Infra.Cache.Interfaces;
using Infra.Cache.Options;
using Microsoft.Extensions.Options;

namespace Infra.Cache.Repositories;

public class ConfigurationCacheRepository : BaseRedisCacheRepository, IConfigurationRepository
{
    private readonly IConfigurationRepository _configurationRepository;

    public ConfigurationCacheRepository(
        IConfigurationRepository configurationRepository,
        IRedisConnectionManager connectionManager,
        IOptionsSnapshot<CacheOptions> cacheOptions)
        : base(
            connectionManager,
            cacheOptions.Value,
            AppConstants.AppName,
            nameof(Configuration))
    {
        _configurationRepository = configurationRepository;
    }

    public async Task<Configuration> Get(string configurationId, CancellationToken cancellationToken = default)
    {
        var configuration = await GetOrRefreshCache(configurationId,
            () => _configurationRepository.Get(configurationId, cancellationToken)
        );

        return configuration;
    }

    public async Task<PaginatedResult<Configuration>> GetAll(PaginationRequest request, CancellationToken cancellationToken = default)
    {
        return await _configurationRepository.GetAll(request, cancellationToken);
    }

    public async Task<bool> Save(Configuration configuration, CancellationToken cancellationToken = default)
    {
        return await _configurationRepository.Save(configuration, cancellationToken);
    }

    public async Task<bool> Update(string configurationId, ConfigurationRequest configuration, CancellationToken cancellationToken = default)
    {
        return await _configurationRepository.Update(configurationId, configuration, cancellationToken);
    }
}
