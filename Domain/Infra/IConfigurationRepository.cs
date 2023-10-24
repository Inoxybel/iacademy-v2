using CrossCutting.Helpers;
using Domain.DTO.Configuration;
using Domain.Entities.Configuration;

namespace Domain.Infra;

public interface IConfigurationRepository
{
    Task<Configuration> Get(string configurationId, CancellationToken cancellationToken = default);
    Task<PaginatedResult<Configuration>> GetAll(PaginationRequest request, CancellationToken cancellationToken = default);
    Task<bool> Save(Configuration configuration, CancellationToken cancellationToken = default);
    Task<bool> Update(string configurationId, ConfigurationRequest configuration, CancellationToken cancellationToken = default);
}