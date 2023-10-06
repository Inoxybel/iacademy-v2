using Domain.Entities.Companies;

namespace Domain.Infra;

public interface ICompanyRepository
{
    Task<Company> GetByRef(string cnpj, CancellationToken cancellationToken = default);
}
