using Domain.DTO.Summary;
using Domain.Entities;
using Domain.Infra;
using MongoDB.Driver;

namespace Infra.Repositories;

public class SummaryRepository : ISummaryRepository
{
    private readonly DbContext _dbContext;
    public SummaryRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Summary> Get(string id, CancellationToken cancellationToken = default)
    {
        return await (await _dbContext.Summary.FindAsync(s => s.Id == id, cancellationToken: cancellationToken))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<Summary>> GetAllByOwnerId(
        string ownerId,
        bool isAvaliable = false,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Summary.FindSync(s => s.OwnerId == ownerId && s.IsAvaliable == isAvaliable, cancellationToken: cancellationToken)
            .ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<List<Summary>> GetAllByCategory(
        string category,
        bool isAvaliable = false,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Summary.FindSync(s => s.Category == category && s.IsAvaliable == isAvaliable, cancellationToken: cancellationToken)
            .ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<List<Summary>> GetAllBySubcategory(
        string subcategory,
        bool isAvaliable = false,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Summary.FindSync(s => s.Subcategory == subcategory && s.IsAvaliable == isAvaliable, cancellationToken: cancellationToken)
            .ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<List<Summary>> GetAllByCategoryAndSubcategory(
        string category,
        string subcategory,
        bool isAvaliable = false,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Summary.FindSync(s => s.Category == category && s.Subcategory == subcategory && s.IsAvaliable == isAvaliable, cancellationToken: cancellationToken)
            .ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<bool> Save(Summary summary, CancellationToken cancellationToken = default)
    {
        try
        {
            await _dbContext.Summary.InsertOneAsync(summary, null, cancellationToken);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> Update(string summaryId, SummaryRequest summary, CancellationToken cancellationToken = default)
    {
        try
        {
            var filterDefinition = Builders<Summary>.Filter.Eq(s => s.Id, summaryId);

            var updateDefinition = Builders<Summary>.Update
                .Set(s => s.OriginId, summary.OriginId)
                .Set(s => s.OwnerId, summary.OwnerId)
                .Set(s => s.ConfigurationId, summary.ConfigurationId)
                .Set(s => s.ChatId, summary.ChatId)
                .Set(s => s.IsAvaliable, summary.IsAvaliable)
                .Set(s => s.Category, summary.Category)
                .Set(s => s.Subcategory, summary.Subcategory)
                .Set(s => s.Theme, summary.Theme)
                .Set(s => s.UpdatedDate, DateTime.UtcNow)
                .Set(s => s.Topics, summary.Topics);

            var result = await _dbContext.Summary.UpdateOneAsync(filterDefinition, updateDefinition, null, cancellationToken);

            return result.ModifiedCount > 0;
        }
        catch
        {
            return false;
        }
    }
}