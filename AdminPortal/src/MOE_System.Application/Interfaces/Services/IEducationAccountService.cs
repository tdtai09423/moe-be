namespace MOE_System.Application.Interfaces.Services;

public interface IEducationAccountService
{
    Task CloseEducationAccountsAsync(string? nric, CancellationToken cancellationToken = default);
}