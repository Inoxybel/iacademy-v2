using System.ComponentModel.DataAnnotations;

namespace Domain.DTO.Summary
{
    public class SummaryMatriculationRequest
    {
        [Required]
        public string SummaryId { get; set; }
        [Required]
        public string OwnerId { get; set; }
    }
}
