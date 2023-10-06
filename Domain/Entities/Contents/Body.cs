namespace Domain.Entities.Contents;

public class Body
{
    public List<Subcontent> Contents { get; set; }

    public string GetAllActiveContent() =>
        string.Join(" ", Contents
            .SelectMany(sc => sc.SubcontentHistory)
            .Where(h => h.DisabledDate == DateTime.MinValue)
            .Select(h => h.Content)
        );
}
