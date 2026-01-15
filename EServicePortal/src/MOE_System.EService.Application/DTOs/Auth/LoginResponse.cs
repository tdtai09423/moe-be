namespace MOE_System.EService.Application.DTOs.Auth;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string EducationAccountId { get; set; } = string.Empty;
    public string AccountHolderId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}
