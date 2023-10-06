using Domain.Entities.Companies;
using Domain.Infra;
using MongoDB.Driver;

namespace Infra.Repositories;

public class CompanyRepository : ICompanyRepository
{
    private readonly DbContext _dbContext;

    public CompanyRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Company> GetByRef(string cnpj, CancellationToken cancellationToken = default)
    {
        var company = await _dbContext.Company.Find(c => c.Cnpj == cnpj).FirstOrDefaultAsync(cancellationToken);

        return company;
    }
        
}
