using CrossCutting.Helpers;
using Domain.DTO.Configuration;
using Domain.Entities.Configuration;
using Domain.Infra;
using MongoDB.Driver;
using System.Collections.Generic;

namespace Infra.Repositories;

public class ConfigurationRepository : IConfigurationRepository
{
    private readonly DbContext _dbContext;

    public ConfigurationRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Configuration> Get(string id, CancellationToken cancellationToken = default)
    {
        return await (await _dbContext.Configuration.FindAsync(c => c.Id == id, cancellationToken: cancellationToken))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<PaginatedResult<Configuration>> GetAll(PaginationRequest pagination, CancellationToken cancellationToken)
    {
        var filter = FilterDefinition<Configuration>.Empty;

        var totalRecords = await _dbContext.Configuration.CountDocumentsAsync(filter, cancellationToken: cancellationToken);

        var data = await _dbContext.Configuration.Find(filter)
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Limit(pagination.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<Configuration>
        {
            Data = data,
            TotalRecords = totalRecords,
            Page = pagination.PageNumber,
            PageSize = pagination.PageSize
        };
    }

    public async Task<bool> Save(Configuration content, CancellationToken cancellationToken = default)
    {
        try
        {
            await _dbContext.Configuration.InsertOneAsync(content, cancellationToken: cancellationToken);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> Update(string configurationId, ConfigurationRequest configuration, CancellationToken cancellationToken = default)
    {
        try
        {
            var filterDefinition = Builders<Configuration>.Filter.Eq(c => c.Id, configurationId);

            var filterUpdate = Builders<Configuration>.Update
                .Set(c => c.Name, configuration.Name)
                .Set(c => c.Summary, configuration.Summary)
                .Set(c => c.FirstContent, configuration.FirstContent)
                .Set(c => c.NewContent, configuration.NewContent)
                .Set(c => c.Correction, configuration.Correction)
                .Set(c => c.Exercise, configuration.Exercise)
                .Set(c => c.Pendency, configuration.Pendency);

            var result = await _dbContext.Configuration.UpdateOneAsync(filterDefinition, filterUpdate, null, cancellationToken);

            return result.ModifiedCount > 0;
        }
        catch
        {
            return false;
        }
    }
}