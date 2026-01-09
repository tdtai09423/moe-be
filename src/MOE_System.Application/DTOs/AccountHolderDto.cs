namespace MOE_System.Application.DTOs;

public class AccountHolderDto
{
    public string Id { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public DateTime DateOfBirth { get; set; }
    public int Age => DateTime.Now.Year - DateOfBirth.Year;
    public string Address { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string ContactNumber { get; set; } = string.Empty;
    public string NRIC { get; set; } = string.Empty;
    public string CitizenId { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string ContLearningStatus { get; set; } = string.Empty;
    public string EducationLevel { get; set; } = string.Empty;
    public string SchoolingStatus { get; set; } = string.Empty;
    public EducationAccountInfoDto? EducationAccount { get; set; }
}

public class EducationAccountInfoDto
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public bool IsActive { get; set; }
    public DateTime? LastLoginAt { get; set; }
}
