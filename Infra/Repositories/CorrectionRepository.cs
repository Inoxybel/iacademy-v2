using Domain.Entities;
using Domain.Infra;
using MongoDB.Driver;

namespace Infra.Repositories;

public class CorrectionRepository : ICorrectionRepository
{
    private readonly DbContext _dbContext;

    public CorrectionRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Correction> Get(string correctionId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Correction>.Filter.Eq(c => c.Id, correctionId);

        return await _dbContext.Correction.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> Save(Correction correction, CancellationToken cancellationToken = default)
    {
        try
        {
            await _dbContext.Correction.InsertOneAsync(correction, cancellationToken: cancellationToken);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> Update(string correctionId, Correction correction, CancellationToken cancellationToken = default)
    {
        try
        {
            var filterDefinition = Builders<Correction>.Filter.Eq(e => e.Id, correctionId);

            var updateDefinition = Builders<Correction>.Update
                .Set(s => s.ExerciseId, correction.ExerciseId)
                .Set(s => s.OwnerId, correction.OwnerId)
                .Set(s => s.UpdatedDate, correction.UpdatedDate)
                .Set(s => s.Corrections, correction.Corrections);

            var result = await _dbContext.Correction.UpdateOneAsync(filterDefinition, updateDefinition, null, cancellationToken);

            return result.ModifiedCount > 0;
        }
        catch
        {
            return false;
        }
    }
}