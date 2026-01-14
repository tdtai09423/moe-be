namespace MOE_System.EService.Application.Interfaces.Services;

public interface IJwtService
{
    string GenerateToken(string accountHolderId, string userName, string email);
}
