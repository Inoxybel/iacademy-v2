using Domain.DTO.Configuration;
using Domain.DTO;
using Domain.Entities.Configuration;

namespace Domain.Services;

public interface IConfigurationService
{
    Task<ServiceResult<Configuration>> Get(string configurationId, CancellationToken cancellationToken = default);
    Task<ServiceResult<ConfigurationResponse>> Create(ConfigurationRequest configuration, CancellationToken cancellationToken = default);
    Task<ServiceResult<bool>> Update(string configurationId, ConfigurationRequest configuration, CancellationToken cancellationToken = default);
}