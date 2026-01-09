namespace MOE_System.EService.Application.DTOs
{
    public class EducationAccountBalanceResponse
    {
        public string EducationAccountId { get; set; } = string.Empty;
        public string AccountHolderId { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public bool IsActive { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
