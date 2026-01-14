using MOE_System.Domain.Common;

namespace MOE_System.Domain.Entities;

public class AccountHolder : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string RegisteredAddress { get; set; } = string.Empty;
    public string MailingAddress { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty; // Combined address field
    public string Email { get; set; } = string.Empty;
    public string ContactNumber { get; set; } = string.Empty;
    public string NRIC { get; set; } = string.Empty;
    public string CitizenId { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string ContLearningStatus { get; set; } = string.Empty;
    public string EducationLevel { get; set; } = string.Empty;
    public string SchoolingStatus { get; set; } = string.Empty;
    public string ResidentialStatus { get; set; } = string.Empty;


    // Navigation property (1-to-1)                                            
    public EducationAccount? EducationAccount { get; set; }
}
