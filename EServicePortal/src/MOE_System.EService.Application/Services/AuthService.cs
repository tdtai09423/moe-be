using Microsoft.EntityFrameworkCore;
using MOE_System.EService.Domain.Common;
using MOE_System.EService.Domain.Entities;
using MOE_System.EService.Application.Common.Interfaces;
using MOE_System.EService.Application.DTOs.Auth;
using MOE_System.EService.Application.Interfaces;
using MOE_System.EService.Application.Interfaces.Services;

namespace MOE_System.EService.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly IPasswordService _passwordService;

    public AuthService(IUnitOfWork unitOfWork, IJwtService jwtService, IPasswordService passwordService)
    {
        _unitOfWork = unitOfWork;
        _jwtService = jwtService;
        _passwordService = passwordService;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var educationAccountRepo = _unitOfWork.GetRepository<EducationAccount>();

        var educationAccount = await educationAccountRepo.FindAsync(
            x => x.UserName.ToLower() == request.UserName.ToLower(),
            q => q.Include(x => x.AccountHolder)
        );

        if (educationAccount == null)
        {
            throw new BaseException.NotFoundException("Invalid username or password");
        }

        // Verify password
        // if (!_passwordService.VerifyPassword(request.Password, educationAccount.Password))
        // {
        //     throw new BaseException.NotFoundException("Invalid username or password");
        // }

        // Check if account is active
        if (!educationAccount.IsActive)
        {
            throw new BaseException.BadRequestException("Your account is not active. Please contact support.");
        }

        // Update last login time
        educationAccount.LastLoginAt = DateTime.UtcNow;
        await _unitOfWork.SaveAsync();

        // Generate JWT token
        var token = _jwtService.GenerateToken(
            educationAccount.AccountHolderId,
            educationAccount.UserName,
            educationAccount.AccountHolder?.Email ?? ""
        );

        var expirationMinutes = int.Parse("1440"); // Should get from config
        
        return new LoginResponse
        {
            Token = token,
            EducationAccountId = educationAccount.Id,
            AccountHolderId = educationAccount.AccountHolderId,
            FullName = $"{educationAccount.AccountHolder?.FirstName} {educationAccount.AccountHolder?.LastName}",
            Email = educationAccount.AccountHolder?.Email ?? "",
            ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes)
        };
    }
}
