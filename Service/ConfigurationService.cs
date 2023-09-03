using Domain.DTO;
using Domain.DTO.Configuration;
using Domain.Entities;
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
            return new()
            {
                Success = false,
                ErrorMessage = "Configuration not finded."
            };

        return new()
        {
            Success = true,
            Data = configuration
        };
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

        bool isSuccess = await _repository.Save(newConfiguration, cancellationToken);

        return new()
        {
            Success = isSuccess,
            ErrorMessage = isSuccess ? string.Empty : "Error on save configuration",
            Data = isSuccess ? new ConfigurationResponse { Id = newConfiguration.Id } : null
        };
    }

    public async Task<ServiceResult<bool>> Update(string configurationId, ConfigurationRequest configuration, CancellationToken cancellationToken = default)
    {
        var existingConfiguration = await _repository.Get(configurationId, cancellationToken);

        if (existingConfiguration == null)
            return new()
            {
                Success = false,
                ErrorMessage = "Configuration not finded."
            };

        var isSuccess = await _repository.Update(configurationId, existingConfiguration, cancellationToken);

        return new()
        {
            Success = isSuccess,
            ErrorMessage = isSuccess ? string.Empty : "Error on update configuration",
            Data = isSuccess
        };
    }
}