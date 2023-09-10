using Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Domain.DTO.Configuration;

public class ConfigurationRequest
{
    [Required]
    public InputProperties Summary { get; set; }
    [Required]
    public InputProperties FirstContent { get; set; }
    [Required]
    public InputProperties NewContent { get; set; }
    [Required]
    public InputProperties Exercise { get; set; }
    [Required]
    public InputProperties Correction { get; set; }
    [Required]
    public InputProperties Pendency { get; set; }
}

