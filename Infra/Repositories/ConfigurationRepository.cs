using Domain.Entities;
using Domain.Infra;
using MongoDB.Driver;

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

    public async Task<bool> Update(string configurationId, Configuration configuration, CancellationToken cancellationToken = default)
    {
        try
        {
            var filterDefinition = Builders<Configuration>.Filter.Eq(c => c.Id, configurationId);

            var filterUpdate = Builders<Configuration>.Update
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