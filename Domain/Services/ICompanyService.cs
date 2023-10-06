using Domain.Entities.Companies;

namespace Domain.Services;

public interface ICompanyService
{
    Task<Company> GetByRef(string cnpj, CancellationToken cancellationToken = default);
}
