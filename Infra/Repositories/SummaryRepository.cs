using CrossCutting.Helpers;
using Domain.DTO.Summary;
using Domain.Entities.Summary;
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

    public async Task<Summary> Get(string summaryId, CancellationToken cancellationToken = default) =>
        await (await _dbContext.Summary.FindAsync(s => s.Id == summaryId, cancellationToken: cancellationToken))
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<PaginatedResult<Summary>> GetAllByOwnerId(
        PaginationRequest pagination,
        string ownerId,
        bool isAvaliable = true,
        CancellationToken cancellationToken = default)
    {
        var filter = Builders<Summary>.Filter.Where(s => s.OwnerId == ownerId && s.IsAvaliable == isAvaliable);

        var totalRecords = await _dbContext.Summary.CountDocumentsAsync(filter, cancellationToken: cancellationToken);

        var data = await _dbContext.Summary.Find(filter)
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Limit(pagination.PageSize)
            .ToListAsync(cancellationToken: cancellationToken);

        return new PaginatedResult<Summary>
        {
            Data = data,
            TotalRecords = totalRecords,
            Page = pagination.PageNumber,
            PageSize = pagination.PageSize
        };
    }

    public async Task<PaginatedResult<SummaryResumeResponse>> GetAllAvailableToCompany(
        PaginationRequest pagination,
        string ownerId,
        CancellationToken cancellationToken = default)
    {
        var filter = Builders<Summary>.Filter.Where(s => s.OwnerId == ownerId);

        var totalRecords = await _dbContext.Summary.CountDocumentsAsync(filter, cancellationToken: cancellationToken);

        var data = await _dbContext.Summary.Find(filter)
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Limit(pagination.PageSize)
            .ToListAsync(cancellationToken: cancellationToken);

        var mappedData = data.Select(MapToResponse).ToList();

        return new PaginatedResult<SummaryResumeResponse>
        {
            Data = mappedData,
            TotalRecords = totalRecords,
            Page = pagination.PageNumber,
            PageSize = pagination.PageSize
        };
    }

    public async Task<PaginatedResult<Summary>> GetAllByIds(
        PaginationRequest pagination,
        List<string> summaryIds, 
        bool isAvaliable = true, 
        CancellationToken cancellationToken = default)
    {
        var filter = Builders<Summary>.Filter.Where(s => summaryIds.Contains(s.Id) && s.IsAvaliable == isAvaliable);

        var totalRecords = await _dbContext.Summary.CountDocumentsAsync(filter, cancellationToken: cancellationToken);

        var data = await _dbContext.Summary.Find(filter)
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Limit(pagination.PageSize)
            .ToListAsync(cancellationToken: cancellationToken);

        return new PaginatedResult<Summary>
        {
            Data = data,
            TotalRecords = totalRecords,
            Page = pagination.PageNumber,
            PageSize = pagination.PageSize
        };
    }

    public async Task<PaginatedResult<Summary>> GetAllByCategory(
        PaginationRequest pagination,
        List<string> summaryIds,
        string category,
        bool isAvaliable = true,
        CancellationToken cancellationToken = default)
    {
        var filter = Builders<Summary>.Filter.Where(s => summaryIds.Contains(s.Id) && s.Category == category && s.IsAvaliable == isAvaliable);

        var totalRecords = await _dbContext.Summary.CountDocumentsAsync(filter, cancellationToken: cancellationToken);

        var data = await _dbContext.Summary.Find(filter)
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Limit(pagination.PageSize)
            .ToListAsync(cancellationToken: cancellationToken);

        return new PaginatedResult<Summary>
        {
            Data = data,
            TotalRecords = totalRecords,
            Page = pagination.PageNumber,
            PageSize = pagination.PageSize
        };
    }

    public async Task<PaginatedResult<Summary>> GetAllBySubcategory(
        PaginationRequest pagination,
        List<string> summaryIds,
        string subcategory,
        bool isAvaliable = true,
        CancellationToken cancellationToken = default)
    {
        var filter = Builders<Summary>.Filter.Where(s => summaryIds.Contains(s.Id) && s.Subcategory == subcategory && s.IsAvaliable == isAvaliable);

        var totalRecords = await _dbContext.Summary.CountDocumentsAsync(filter, cancellationToken: cancellationToken);

        var data = await _dbContext.Summary.Find(filter)
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Limit(pagination.PageSize)
            .ToListAsync(cancellationToken: cancellationToken);

        return new PaginatedResult<Summary>
        {
            Data = data,
            TotalRecords = totalRecords,
            Page = pagination.PageNumber,
            PageSize = pagination.PageSize
        };
    }

    public async Task<PaginatedResult<Summary>> GetAllByCategoryAndSubcategory(
        PaginationRequest pagination,
        List<string> summaryIds,
        string category,
        string subcategory,
        bool isAvaliable = true,
        CancellationToken cancellationToken = default)
    {
        var filter = Builders<Summary>.Filter.Where(s => summaryIds.Contains(s.Id) && s.Category == category && s.Subcategory == subcategory && s.IsAvaliable == isAvaliable);

        var totalRecords = await _dbContext.Summary.CountDocumentsAsync(filter, cancellationToken: cancellationToken);

        var data = await _dbContext.Summary.Find(filter)
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Limit(pagination.PageSize)
            .ToListAsync(cancellationToken: cancellationToken);

        return new PaginatedResult<Summary>
        {
            Data = data,
            TotalRecords = totalRecords,
            Page = pagination.PageNumber,
            PageSize = pagination.PageSize
        };
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
                .Set(s => s.Icon, summary.Icon)
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

    public async Task<bool> IsEnrolled(string summaryId, string ownerId, CancellationToken cancellationToken = default)
    {
        var result = await (await _dbContext.Summary
            .FindAsync(
                s => s.OriginId == summaryId && s.OwnerId == ownerId,
                cancellationToken: cancellationToken
             )
        ).FirstOrDefaultAsync(cancellationToken);

        return result is not null;
    }

    public async Task<List<string>> IsEnrolled(List<string> summaryIds, string ownerId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Summary>.Filter.In(s => s.OriginId, summaryIds) &
                     Builders<Summary>.Filter.Eq(s => s.OwnerId, ownerId);

        var summaries = await _dbContext.Summary
            .Find(filter)
            .ToListAsync(cancellationToken);

        if (summaries is null || !summaries.Any())
            return new List<string>();

        return summaries.Select(s => s.OriginId).ToList();
    }

    public async Task<bool> ShouldGeneratePendency(string summaryId, string ownerId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Summary>.Filter.Eq(s => s.Id, summaryId) &
                     Builders<Summary>.Filter.Eq(s => s.OwnerId, ownerId);

        var summary = await _dbContext.Summary
            .FindSync(filter, null, cancellationToken).FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (summary is not null)
            return summary.ShouldGeneratePendency;

        return false;
    }

    private SummaryResumeResponse MapToResponse(Summary summary)
    {
        return new SummaryResumeResponse
        {
            Id = summary.Id,
            Icon = summary.Icon,
            IsAvaliable = summary.IsAvaliable,
            Category = summary.Category,
            ShouldGeneratePendency = summary.ShouldGeneratePendency,
            Subcategory = summary.Subcategory,
            Theme = summary.Theme,
            Topics = summary.Topics.Select(t => new TopicResumeDTO
            {
                Index = t.Index,
                Title = t.Title,
                Description = t.Description,
                Subtopics = t.Subtopics.Select(st => new SubtopicResumeDTO
                {
                    Index = st.Index,
                    Title = st.Title
                }).ToList()
            }).ToList()
        };
    }

}