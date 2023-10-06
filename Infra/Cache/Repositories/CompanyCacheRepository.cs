using CrossCutting.Constants;
using Domain.Entities.Companies;
using Domain.Infra;
using Infra.Cache.Interfaces;
using Infra.Cache.Options;
using Microsoft.Extensions.Options;

namespace Infra.Cache.Repositories;

public class CompanyCacheRepository : BaseRedisCacheRepository, ICompanyRepository
{
    private readonly ICompanyRepository _companyRepository;

    public CompanyCacheRepository(
        ICompanyRepository companyRepository,
        IRedisConnectionManager redisConnectionManager,
        IOptionsSnapshot<CacheOptions> cacheOptions)
        : base(
            redisConnectionManager,
            cacheOptions.Value,
            AppConstants.AppName,
            nameof(Company))
    {
        _companyRepository = companyRepository;
    }

    public async Task<Company> GetByRef(string cnpj, CancellationToken cancellationToken = default)
    {
        var company = await GetOrRefreshCache(cnpj,
            () => _companyRepository.GetByRef(cnpj, cancellationToken)
        );

        return company;
    }
}
