namespace MOE_System.Application.DTOs;

public class AccountBalanceDto
{
    public string AccountId { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public bool IsActive { get; set; }
    public DateTime? LastLoginAt { get; set; }
}
