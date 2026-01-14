using System.ComponentModel.DataAnnotations;

namespace MOE_System.Application.DTOs.AccountHolder.Request;

public class BulkAccountOperationRequest
{
    [Required]
    [MinLength(1, ErrorMessage = "At least one account ID must be provided")]
    public List<string> AccountIds { get; set; } = new List<string>();
}
