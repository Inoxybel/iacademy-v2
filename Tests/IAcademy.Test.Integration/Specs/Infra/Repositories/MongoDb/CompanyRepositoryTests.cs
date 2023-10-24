using Domain.Entities.Companies;
using Domain.Infra;
using FluentAssertions;
using IAcademy.Test.Integration.Base;
using IAcademy.Test.Integration.Configuration;
using IAcademy.Test.Shared.Builders;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Xunit.Abstractions;

namespace IAcademy.Test.Integration.Specs.Infra.Repositories.MongoDb;

[Collection(Constants.WEB_API_TEST_COLLECTION_NAME)]
public class CompanyRepositoryTests : IntegrationTestBase
{
    private readonly IntegrationTestsFixture _fixture;
    private readonly ITestOutputHelper _output;

    public CompanyRepositoryTests(IntegrationTestsFixture fixture, ITestOutputHelper output)
        : base(fixture)
    {
        _fixture = fixture;
        _output = output;
    }

    [Fact]
    public async Task ShouldBeReturnCompany()
    {
        await _fixture.DbContext.DropCollection(nameof(Company));

        var company = new CompanyBuilder()
            .WithId(Guid.NewGuid().ToString())
            .WithCnpj("CNPJ")
            .Build();

        _output.WriteLine($"Before insertion: {JsonConvert.SerializeObject(company)}");

        await _fixture.DbContext.Company.InsertOneAsync(company);

        var result = await _fixture.serviceProvider
            .GetRequiredService<ICompanyRepository>().GetByRef(company.Cnpj);

        _output.WriteLine($"After retrieval: {JsonConvert.SerializeObject(result)}");

        result.Should().BeEquivalentTo(company);
    }
}
