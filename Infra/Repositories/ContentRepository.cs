using Domain.DTO.Content;
using Domain.Entities.Contents;
using Domain.Infra;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Infra.Repositories;

public class ContentRepository : IContentRepository
{
    private readonly DbContext _dbContext;

    public ContentRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Content> Get(string id, CancellationToken cancellationToken = default)
    {
        return await (await _dbContext.Content.FindAsync(c => c.Id == id, cancellationToken: cancellationToken))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<Content>> GetAllByIds(IEnumerable<string> ids, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Content>.Filter.In(c => c.Id, ids);

        var cursor = await _dbContext.Content.FindAsync(filter, null, cancellationToken);

        return await cursor.ToListAsync(cancellationToken);
    }

    public async Task<List<Content>> GetAllBySummaryId(string summaryId, CancellationToken cancellationToken = default)
    {
        return await (await _dbContext.Content.FindAsync(c => c.SummaryId == summaryId, cancellationToken: cancellationToken))
            .ToListAsync(cancellationToken);
    }

    public async Task<string> Save(Content content, CancellationToken cancellationToken = default)
    {
        try
        {
            var filter = Builders<Content>.Filter.Eq(c => c.Id, content.Id);
            var options = new ReplaceOptions 
            { 
                IsUpsert = true 
            };

            content.UpdatedDate = DateTime.UtcNow;

            _ = await _dbContext.Content.ReplaceOneAsync(filter, content, options, cancellationToken);
            return content.Id;
        }
        catch
        {
            return string.Empty;
        }
    }

    public async Task<List<string>> SaveAll(List<Content> contents, CancellationToken cancellationToken = default)
    {
        try
        {
            var bulkOperations = contents.Select(content =>
                new ReplaceOneModel<Content>(
                    filter: new BsonDocument("_id", content.Id),
                    replacement: content)
                {
                    IsUpsert = true
                }).ToList();

            await _dbContext.Content.BulkWriteAsync(bulkOperations, cancellationToken: cancellationToken);

            return contents.Select(c => c.Id).ToList();
        }
        catch
        {
            return new List<string>();
        }
    }


    public async Task<bool> Update(string contentId, ContentRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var filterDefinition = Builders<Content>.Filter.Eq(c => c.Id, contentId);
            filterDefinition &= Builders<Content>.Filter.Eq(c => c.OwnerId, request.OwnerId);

            var filterUpdate = Builders<Content>.Update
                .Set(c => c.OwnerId, request.OwnerId)
                .Set(c => c.ConfigurationId, request.ConfigurationId)
                .Set(c => c.SummaryId, request.SummaryId)
                .Set(c => c.ExerciseId, request.ExerciseId)
                .Set(c => c.SubtopicIndex, request.SubtopicIndex)
                .Set(c => c.Title, request.Title)
                .Set(c => c.Theme, request.Theme)
                .Set(c => c.Body, request.Body)
                .Set(c => c.UpdatedDate, DateTime.UtcNow);

            var result = await _dbContext.Content.UpdateOneAsync(filterDefinition, filterUpdate, null, cancellationToken);

            return result.ModifiedCount > 0;
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
                    .Set(c => c.ConfigurationId, content.ConfigurationId)
                    .Set(c => c.SummaryId, content.SummaryId)
                    .Set(c => c.ExerciseId, content.ExerciseId)
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

            return result.ModifiedCount > 0;
        }
        catch
        {
            return false;
        }
    }
}