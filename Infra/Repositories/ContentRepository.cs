using Domain.DTO.Content;
using Domain.Entities;
using Domain.Infra;
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

    public async Task<List<Content>> GetAllBySummaryId(string summaryId, CancellationToken cancellationToken = default)
    {
        return await (await _dbContext.Content.FindAsync(c => c.SummaryId == summaryId, cancellationToken: cancellationToken))
            .ToListAsync(cancellationToken);
    }

    public async Task<string> Save(Content content, CancellationToken cancellationToken = default)
    {
        try
        {
            await _dbContext.Content.InsertOneAsync(content, cancellationToken: cancellationToken);
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
            await _dbContext.Content.InsertManyAsync(contents, null, cancellationToken);
            return contents.Select(c => c.Id).ToList();
        }
        catch
        {
            return new();
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
                .Set(c => c.ExerciceId, request.ExerciceId)
                .Set(c => c.SubtopicIndex, request.SubtopicIndex)
                .Set(c => c.Title, request.Title)
                .Set(c => c.Theme, request.Theme)
                .Set(c => c.Body, request.Body)
                .Set(c => c.UpdatedDate, DateTime.UtcNow);

            var result = await _dbContext.Content.UpdateOneAsync(filterDefinition, filterUpdate, null, cancellationToken);

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
                    .Set(c => c.ConfigurationId, content.ConfigurationId)
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