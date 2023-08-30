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
            var filterDefinition = Builders<Configuration>.Filter.Eq(c => c.Id, configuration.Id);

            var filterUpdate = Builders<Configuration>.Update
                .Set(c => c.Summary, configuration.Summary)
                .Set(c => c.FirstContent, configuration.FirstContent)
                .Set(c => c.NewContent, configuration.NewContent)
                .Set(c => c.Correction, configuration.Correction)
                .Set(c => c.Exercise, configuration.Exercise)
                .Set(c => c.Pendency, configuration.Pendency);

            var result = await _dbContext.Configuration.UpdateOneAsync(filterDefinition, filterUpdate, null, cancellationToken);

            if (result.ModifiedCount > 0)
                return true;

            return false;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateAll(string summaryId, List<Content> contents, CancellationToken cancellationToken = default)
    {
        try
        {
            var requestsToUpdate = new List<WriteModel<Content>>();

            foreach (var content in contents)
            {
                var filter = Builders<Content>.Filter.Eq(c => c.SummaryId, summaryId);
                var update = Builders<Content>.Update
                    .Set(c => c.OwnerId, content.OwnerId)
                    .Set(c => c.SummaryId, content.SummaryId)
                    .Set(c => c.ExerciceId, content.ExerciceId)
                    .Set(c => c.SubtopicIndex, content.SubtopicIndex)
                    .Set(c => c.Title, content.Title)
                    .Set(c => c.Body, content.Body)
                    .Set(c => c.UpdatedDate, content.UpdatedDate);

                var upsert = new UpdateOneModel<Content>(filter, update)
                {
                    IsUpsert = false
                };

                requestsToUpdate.Add(upsert);
            }

            var result = await _dbContext.Content.BulkWriteAsync(requestsToUpdate, cancellationToken: cancellationToken);

            if (result.ModifiedCount > 0)
                return true;

            return false;
        }
        catch
        {
            return false;
        }
    }
}