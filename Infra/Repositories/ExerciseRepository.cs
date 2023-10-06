using CrossCutting.Enums;
using Domain.Entities.Exercise;
using Domain.Infra;
using MongoDB.Driver;

namespace Infra.Repositories;

public class ExerciseRepository : IExerciseRepository
{
    private readonly DbContext _dbContext;

    public ExerciseRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Exercise> Get(string exerciseId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Exercise>.Filter.Eq(e => e.Id, exerciseId);
        return await _dbContext.Exercise.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<Exercise>> GetAllByIds(IEnumerable<string> ids, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Exercise>.Filter.In(c => c.Id, ids);

        var cursor = await _dbContext.Exercise.FindAsync(filter, null, cancellationToken);

        return await cursor.ToListAsync(cancellationToken);
    }

    public async Task<List<Exercise>> GetAllByOwnerIdAndType(string ownerId, ExerciseType type, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Exercise>.Filter.And(
            Builders<Exercise>.Filter.Eq(e => e.OwnerId, ownerId),
            Builders<Exercise>.Filter.Eq(e => e.Type, type)
        );

        return await _dbContext.Exercise.Find(filter).ToListAsync(cancellationToken);
    }

    public async Task<bool> Save(Exercise exercise, CancellationToken cancellationToken = default)
    {
        try
        {
            var filter = Builders<Exercise>.Filter.Eq(c => c.Id, exercise.Id);
            var options = new ReplaceOptions
            {
                IsUpsert = true
            };

            _ = await _dbContext.Exercise.ReplaceOneAsync(filter, exercise, options, cancellationToken: cancellationToken);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<List<string>> SaveAll(IEnumerable<Exercise> exercises, CancellationToken cancellationToken = default)
    {
        try
        {
            await _dbContext.Exercise.InsertManyAsync(exercises, null, cancellationToken);
            return exercises.Select(c => c.Id).ToList();
        }
        catch
        {
            return new();
        }
    }

        public async Task<bool> Update(string exerciseId, Exercise exercise, CancellationToken cancellationToken = default)
    {
        try
        {
            var filterDefinition = Builders<Exercise>.Filter.Eq(e => e.Id, exerciseId);

            var updateDefinition = Builders<Exercise>.Update
                .Set(s => s.OwnerId, exercise.OwnerId)
                .Set(s => s.CorrectionId, exercise.CorrectionId)
                .Set(s => s.ConfigurationId, exercise.ConfigurationId)
                .Set(s => s.ContentId, exercise.ContentId)
                .Set(s => s.Status, exercise.Status)
                .Set(s => s.Type, exercise.Type)
                .Set(s => s.SendedAt, exercise.SendedAt)
                .Set(s => s.TopicIndex, exercise.TopicIndex)
                .Set(s => s.Title, exercise.Title)
                .Set(s => s.Exercises, exercise.Exercises);

            var result = await _dbContext.Exercise.UpdateOneAsync(filterDefinition, updateDefinition, null, cancellationToken);

            return result.ModifiedCount > 0;
        }
        catch
        {
            return false;
        }
    }
}