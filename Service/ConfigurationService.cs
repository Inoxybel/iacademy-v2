using Domain.DTO;
using Domain.DTO.Configuration;
using Domain.Entities.Configuration;
using Domain.Infra;
using Domain.Services;

namespace Service;

public class ConfigurationService : IConfigurationService
{
    private readonly IConfigurationRepository _repository;

    public ConfigurationService(IConfigurationRepository repository)
    {
        _repository = repository;
    }

    public async Task<ServiceResult<Configuration>> Get(string configurationId, CancellationToken cancellationToken = default)
    {
        var configuration = await _repository.Get(configurationId, cancellationToken);

        if (configuration is null)
            return ServiceResult<Configuration>.MakeErrorResult("Configuration not finded.");

        return ServiceResult<Configuration>.MakeSuccessResult(configuration);
    }

    public async Task<ServiceResult<ConfigurationResponse>> Create(ConfigurationRequest configuration, CancellationToken cancellationToken = default)
    {
        var newConfiguration = new Configuration
        {
            Id = Guid.NewGuid().ToString(),
            Summary = configuration.Summary,
            FirstContent = configuration.FirstContent,
            NewContent = configuration.NewContent,
            Exercise = configuration.Exercise,
            Correction = configuration.Correction,
            Pendency = configuration.Pendency
        };

        var response = new ConfigurationResponse
        {
            Id = newConfiguration.Id
        };

        bool isSuccess = await _repository.Save(newConfiguration, cancellationToken);

        return isSuccess ? ServiceResult<ConfigurationResponse>.MakeSuccessResult(response) : ServiceResult<ConfigurationResponse>.MakeErrorResult("Error on save configuration");
    }

    public async Task<ServiceResult<bool>> Update(string configurationId, ConfigurationRequest configuration, CancellationToken cancellationToken = default)
    {
        var existingConfiguration = await _repository.Get(configurationId, cancellationToken);

        if (existingConfiguration == null)
            return ServiceResult<bool>.MakeErrorResult("Configuration not finded.");

        var isSuccess = await _repository.Update(configurationId, configuration, cancellationToken);

        return isSuccess ? ServiceResult<bool>.MakeSuccessResult(isSuccess) : ServiceResult<bool>.MakeErrorResult("Error on update configuration");
    }
}