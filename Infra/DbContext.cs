using Domain.Entities.Companies;
using Domain.Entities.Chat;
using Domain.Entities.Configuration;
using Domain.Entities.Contents;
using Domain.Entities.Exercise;
using Domain.Entities.Feedback;
using Domain.Entities.Summary;
using Infra.Options;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Infra;

public class DbContext
{
    private readonly IMongoDatabase _database;
    private readonly IMongoDatabase _userManagerDatabase;
    public IMongoCollection<Summary> Summary { get; }
    public IMongoCollection<Configuration> Configuration { get; }
    public IMongoCollection<Content> Content { get; }
    public IMongoCollection<Correction> Correction { get; }
    public IMongoCollection<Exercise> Exercise { get; }
    public IMongoCollection<ChatCompletion> ChatCompletion { get; }
    public IMongoCollection<Company> Company { get; }

    protected DbContext() { }

    public DbContext(IOptions<DatabaseInstanceOptions> databaseInstanceOptions)
    {
        var client = new MongoClient(databaseInstanceOptions.Value.ConnectionString);
        _database = client.GetDatabase(databaseInstanceOptions.Value.Name);

        var userManagerClient = new MongoClient(databaseInstanceOptions.Value.UserManagerConnectionString);
        _userManagerDatabase = userManagerClient.GetDatabase(databaseInstanceOptions.Value.UserManagerDBName);

        Summary = _database.GetCollection<Summary>(nameof(Summary));
        Configuration = _database.GetCollection<Configuration>(nameof(Configuration));
        Content = _database.GetCollection<Content>(nameof(Content));
        Correction = _database.GetCollection<Correction>(nameof(Correction));
        Exercise = _database.GetCollection<Exercise>(nameof(Exercise));
        ChatCompletion = _database.GetCollection<ChatCompletion>(nameof(ChatCompletion));
        Company = _userManagerDatabase.GetCollection<Company>(nameof(Company));
    }

    public async Task DropCollection(string collectionName)
    {
        await _database.DropCollectionAsync(collectionName);
        await _userManagerDatabase.DropCollectionAsync(collectionName);
    }
}