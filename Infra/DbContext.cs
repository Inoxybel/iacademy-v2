using Domain.Entities;
using Infra.Options;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Infra;

public class DbContext
{
    public IMongoCollection<Summary> Summary { get; }
    public IMongoCollection<Configuration> Configuration { get; }
    public IMongoCollection<Content> Content { get; }
    public IMongoCollection<Correction> Correction { get; }
    public IMongoCollection<Exercise> Exercise { get; }

    public IMongoCollection<ChatCompletion> ChatCompletion { get; }

    protected DbContext() { }

    public DbContext(IOptions<DatabaseInstanceOptions> databaseInstanceOptions)
    {
        var client = new MongoClient(databaseInstanceOptions.Value.ConnectionString);

        var database = client.GetDatabase(databaseInstanceOptions.Value.Name);

        Summary = database.GetCollection<Summary>(nameof(Summary));
        Configuration = database.GetCollection<Configuration>(nameof(Configuration));
        Content = database.GetCollection<Content>(nameof(Content));
        Correction = database.GetCollection<Correction>(nameof(Correction));
        Exercise = database.GetCollection<Exercise>(nameof(Exercise));
        ChatCompletion = database.GetCollection<ChatCompletion>(nameof(ChatCompletion));
    }
}