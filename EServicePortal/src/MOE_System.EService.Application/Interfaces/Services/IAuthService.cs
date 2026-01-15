using MOE_System.EService.Application.DTOs.Auth;

namespace MOE_System.EService.Application.Interfaces.Services;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
}
