namespace MOE_System.EService.Application.DTOs.AccountHolder;

public class AccountHolderProfileResponse
{
    public string FullName { get; set; } = string.Empty;
    public string NRIC { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public DateTime AccountCreated { get; set; }
    public string SchoolingStatus { get; set; } = string.Empty;
    public string EducationLevel { get; set; } = string.Empty;
    public string ResidentialStatus { get; set; } = string.Empty;
    public string EmailAddress { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string RegisteredAddress { get; set; } = string.Empty;
    public string MailingAddress { get; set; } = string.Empty;
}
