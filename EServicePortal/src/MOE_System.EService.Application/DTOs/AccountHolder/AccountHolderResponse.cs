namespace MOE_System.EService.Application.DTOs.AccountHolder
{
    public class AccountHolderResponse
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string NRIC { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string SchoolingStatus { get; set; } = string.Empty;
        public string EducationLevel { get; set; } = string.Empty;
        public string RegisteredAddress { get; set; } = string.Empty;
        public string MailingAddress { get; set; } = string.Empty;
        public string EducationAccountId { get; set; } = string.Empty;
        public decimal EducationAccountBalance { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
