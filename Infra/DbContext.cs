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
        var database = client.GetDatabase(databaseInstanceOptions.Value.Name);

        var userManagerClient = new MongoClient(databaseInstanceOptions.Value.UserManagerConnectionString);
        var userManagerDatabase = userManagerClient.GetDatabase(databaseInstanceOptions.Value.UserManagerDBName);

        Summary = database.GetCollection<Summary>(nameof(Summary));
        Configuration = database.GetCollection<Configuration>(nameof(Configuration));
        Content = database.GetCollection<Content>(nameof(Content));
        Correction = database.GetCollection<Correction>(nameof(Correction));
        Exercise = database.GetCollection<Exercise>(nameof(Exercise));
        ChatCompletion = database.GetCollection<ChatCompletion>(nameof(ChatCompletion));
        Company = userManagerDatabase.GetCollection<Company>(nameof(Company));
    }
}