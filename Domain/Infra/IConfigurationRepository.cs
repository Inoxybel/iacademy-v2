﻿using Domain.DTO.Configuration;
using Domain.Entities;

namespace Domain.Infra;

public interface IConfigurationRepository
{
    Task<Configuration> Get(string configurationId, CancellationToken cancellationToken = default);
    Task<bool> Save(Configuration configuration, CancellationToken cancellationToken = default);
    Task<bool> Update(string configurationId, ConfigurationRequest configuration, CancellationToken cancellationToken = default);
}