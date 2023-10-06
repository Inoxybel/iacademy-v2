using Domain.Entities.Companies;
using Domain.Infra;
using Domain.Services;

namespace Service;
public class CompanyService : ICompanyService
{
    private readonly ICompanyRepository _companyRepository;

    public CompanyService(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<Company> GetByRef(string cnpj, CancellationToken cancellationToken = default) =>
        await _companyRepository.GetByRef(cnpj, cancellationToken);
}
