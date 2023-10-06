using Domain.Infra;
using FluentAssertions;
using IAcademy.Test.Integration.Base;
using IAcademy.Test.Integration.Configuration;
using IAcademy.Test.Shared.Builders;
using Microsoft.Extensions.DependencyInjection;

namespace IAcademy.Test.Integration.Specs.Infra.Repositories.MongoDb;

[Collection(Constants.WEB_API_TEST_COLLECTION_NAME)]
public class CompanyRepositoryTests : IntegrationTestBase
{
    private readonly IntegrationTestsFixture _fixture;

    public CompanyRepositoryTests(IntegrationTestsFixture fixture)
        : base(fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task ShouldBeReturnCompany()
    {
        var company = new CompanyBuilder()
            .WithId(Guid.NewGuid().ToString())
            .WithCnpj("CNPJ")
            .Build();

        _fixture.DbContext.Company.InsertOne(company);

        var result = await _fixture.serviceProvider
            .GetRequiredService<ICompanyRepository>().GetByRef(company.Cnpj);

        result.Should().BeEquivalentTo(company);
    }
}
