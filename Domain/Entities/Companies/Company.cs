namespace Domain.Entities.Companies;

public class Company
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Cnpj { get; set; }
    public List<CompanyGroup> Groups { get; set; }

    public int LimitPlan { get; set; }

    public List<CompanyGroup> GetGroupByUserDocument(string userDocument) =>
        Groups.Where(group => group.Users.Any(u => u.Document == userDocument) || group.Users.Any(u => u.Document == "*")).ToList();
}
