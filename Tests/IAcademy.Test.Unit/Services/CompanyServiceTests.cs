using Domain.Entities.Companies;
using Domain.Infra;
using FluentAssertions;
using Moq;
using Service;

namespace IAcademy.Test.Unit.Services;

public class CompanyServiceTests
{
    private readonly Mock<ICompanyRepository> _companyRepositoryMock;
    private readonly CompanyService _companyService;

    public CompanyServiceTests()
    {
        _companyRepositoryMock = new Mock<ICompanyRepository>();
        _companyService = new CompanyService(_companyRepositoryMock.Object);
    }

    [Fact]
    public async Task GetByRef_SHOULD_ReturnCompany_WHEN_CnpjIsValid()
    {
        var cnpj = "12345678901234";
        var expectedCompany = new Company 
        { 
            Id = Guid.NewGuid().ToString(),
            Groups = new(),
            Name = "IAcademy",
            Cnpj = "CNPJ"
        };

        _companyRepositoryMock.Setup(repo => repo.GetByRef(cnpj, It.IsAny<CancellationToken>()))
                              .ReturnsAsync(expectedCompany);

        var result = await _companyService.GetByRef(cnpj);

        result.Should().BeEquivalentTo(expectedCompany);
    }

    [Fact]
    public void GetByRef_SHOULD_ThrowException_WHEN_CnpjIsInvalid()
    {
        var invalidCnpj = "invalid_cnpj";

        Func<Task> act = async () => await _companyService.GetByRef(invalidCnpj);

        act.Should().ThrowAsync<Exception>();
    }
}