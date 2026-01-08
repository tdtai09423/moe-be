namespace MOE_System.Application.Interfaces;

/// <summary>
/// Service for password hashing and verification using BCrypt
/// </summary>
public interface IPasswordService
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hashedPassword);
    bool NeedsRehash(string hashedPassword);
    string GenerateRandomPassword(int length = 12);
}
