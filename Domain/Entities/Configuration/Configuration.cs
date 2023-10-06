namespace Domain.Entities.Configuration;

public class Configuration
{
    public string Id { get; set; }
    public InputProperties Summary { get; set; }
    public InputProperties FirstContent { get; set; }
    public InputProperties NewContent { get; set; }
    public InputProperties NewContentWithChat { get; set; }
    public InputProperties Exercise { get; set; }
    public InputProperties Correction { get; set; }
    public InputProperties Pendency { get; set; }
}
