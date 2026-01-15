namespace MOE_System.Application.Interfaces.Services;

public interface IEducationAccountService
{
    Task AutoCloseEducationAccountsAsync(CancellationToken cancellationToken);
    Task CloseEducationAccountManuallyAsync(string nric, CancellationToken cancellationToken);
}