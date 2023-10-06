using Domain.Entities.Configuration;

namespace Domain.DTO.Configuration;

public class ConfigurationRequest
{
    public InputProperties Summary { get; set; }
    public InputProperties FirstContent { get; set; }
    public InputProperties NewContent { get; set; }
    public InputProperties Exercise { get; set; }
    public InputProperties Correction { get; set; }
    public InputProperties Pendency { get; set; }
}

