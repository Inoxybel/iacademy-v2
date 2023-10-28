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
        Users = new()
        {
            new()
            {
                Document = "*"
            }
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

    public CompanyGroupBuilder WithUsersDocument(List<UserInfo> users)
    {
        group.Users = users;
        return this;
    }

    public CompanyGroupBuilder WithAuthorizedTrainingIds(List<string> authorizedTrainingIds)
    {
        group.AuthorizedTrainingIds = authorizedTrainingIds;
        return this;
    }

    public CompanyGroup Build() => group;
}