using Domain.Entities.Companies;

namespace IAcademy.Test.Shared.Builders;

public class CompanyGroupBuilder
{
    private CompanyGroup group;

    public CompanyGroupBuilder()
    {
        group = CreateDefault();
    }

    private static CompanyGroup CreateDefault() => new CompanyGroup
    {
        GroupName = "defaultGroupName",
        UsersDocument = new List<string>()
        {
            "*"
        },
        AuthorizedTrainingIds = new List<string>()
        {
            "TrainingId"
        }
    };

    public CompanyGroupBuilder WithGroupName(string groupName)
    {
        group.GroupName = groupName;
        return this;
    }

    public CompanyGroupBuilder WithUsersDocument(List<string> usersDocument)
    {
        group.UsersDocument = usersDocument;
        return this;
    }

    public CompanyGroupBuilder WithAuthorizedTrainingIds(List<string> authorizedTrainingIds)
    {
        group.AuthorizedTrainingIds = authorizedTrainingIds;
        return this;
    }

    public CompanyGroup Build() => group;
}