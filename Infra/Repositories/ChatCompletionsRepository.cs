using Domain.Entities;
using Domain.Infra;
using MongoDB.Driver;

namespace Infra.Repositories;

public class ChatCompletionsRepository : IChatCompletionsRepository
{
    private readonly DbContext _dbContext;

    public ChatCompletionsRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ChatCompletion> Get(string chatId, CancellationToken cancellationToken = default)
    { 
        return await (await _dbContext.ChatCompletion.FindAsync(c => c.Id == chatId, cancellationToken: cancellationToken))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<string> Save(ChatCompletion chatCompletion, CancellationToken cancellationToken = default)
    {
        try
        {
            await _dbContext.ChatCompletion.InsertOneAsync(chatCompletion, cancellationToken: cancellationToken);
            return chatCompletion.Id;
        }
        catch
        {
            return string.Empty;
        }
    }
}
