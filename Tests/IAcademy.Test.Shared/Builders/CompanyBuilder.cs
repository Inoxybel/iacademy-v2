using Domain.Entities.Companies;

namespace IAcademy.Test.Shared.Builders;

public class CompanyBuilder
{
    private Company company;

    public CompanyBuilder()
    {
        company = CreateDefault();
    }

    private static Company CreateDefault() => new()
    {
        Id = "defaultId",
        Name = "defaultName",
        Cnpj = "defaultCnpj",
        Groups = new List<CompanyGroup>()
        {
            new CompanyGroupBuilder().Build()
        }
    };

    public CompanyBuilder WithId(string id)
    {
        company.Id = id;
        return this;
    }

    public CompanyBuilder WithName(string name)
    {
        company.Name = name;
        return this;
    }

    public CompanyBuilder WithCnpj(string cnpj)
    {
        company.Cnpj = cnpj;
        return this;
    }

    public CompanyBuilder WithGroups(List<CompanyGroup> groups)
    {
        company.Groups = groups;
        return this;
    }

    public CompanyBuilder WithGroup(CompanyGroup group)
    {
        company.Groups.Add(group);
        return this;
    }

    public Company Build() => company;
}
