using MOE_System.Application.Interfaces;
using BCrypt.Net;

namespace MOE_System.Infrastructure.Services;

public class PasswordService : IPasswordService
{

    private const int WorkFactor = 12;

  
    public string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentNullException(nameof(password), "Password cannot be null or empty");
        }

        return BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentNullException(nameof(password), "Password cannot be null or empty");
        }

        if (string.IsNullOrWhiteSpace(hashedPassword))
        {
            throw new ArgumentNullException(nameof(hashedPassword), "Hashed password cannot be null or empty");
        }

        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
        catch (SaltParseException)
        {
            return false;
        }
    }

    public bool NeedsRehash(string hashedPassword)
    {
        if (string.IsNullOrWhiteSpace(hashedPassword))
        {
            return true;
        }

        try
        {
            return !BCrypt.Net.BCrypt.PasswordNeedsRehash(hashedPassword, WorkFactor);
        }
        catch
        {
            return true;
        }
    }

    public string GenerateRandomPassword(int length = 12)
    {
        const string validChars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz23456789!@#$%^&*()-_=+[]{}|;:,.<>?";
        var random = new Random();
        var passwordChars = new char[length];
        for (int i = 0; i < length; i++)
        {
            passwordChars[i] = validChars[random.Next(validChars.Length)];
        }
        return new string(passwordChars);
    }
}
