using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MOE_System.Application.Common;
using MOE_System.Application.Common.Interfaces;
using MOE_System.Application.Interfaces.Services;
using MOE_System.Domain.Entities;
using static MOE_System.Domain.Common.BaseException;

namespace MOE_System.Application.Services;

public class EducationAccountService : IEducationAccountService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClock _clock;
    private readonly AccountClosureOptions _options;

    public EducationAccountService(IUnitOfWork unitOfWork, IClock clock, IOptions<AccountClosureOptions> options)
    {
        _unitOfWork = unitOfWork;
        _clock = clock;
        _options = options.Value;
    }

    public async Task CloseEducationAccountsAsync(string? nric, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(nric))
        {
            await CloseAccountsByNRICAsync(nric, cancellationToken);
            return;
        }
        await AutoCloseAccountsAsync(cancellationToken);
    }

    private async Task CloseAccountsByNRICAsync(string nric, CancellationToken cancellationToken)
    {
        var accountHolderRepository = _unitOfWork.GetRepository<AccountHolder>();

        var accountToClose = await accountHolderRepository.FirstOrDefaultAsync(
            predicate: ah => ah.NRIC == nric,
            include: query => query.Include(ah => ah.EducationAccount),
            cancellationToken: cancellationToken
        );

        if (accountToClose == null || accountToClose.EducationAccount == null)
        {
            throw new NotFoundException("Account holder or associated education account not found.");
        }
        var now = _clock.UtcNow();

        accountToClose.EducationAccount.CloseAccount();
        accountToClose.EducationAccount.ClosedDate = now;

        await _unitOfWork.SaveAsync();
    }

    private async Task AutoCloseAccountsAsync(CancellationToken cancellationToken)
    {
        if (!_options.Enabled)
        {
            return;
        }

        var today = _clock.TodayInTimeZone(_options.TimeZone);
        var utcNow = _clock.UtcNow();

        if (today.Day != _options.ProcessingDay || today.Month != _options.ProcessingMonth)
        {
            return;
        }

        var accountHolderRepository = _unitOfWork.GetRepository<AccountHolder>();

        var accountHolders = await accountHolderRepository.ToListAsync
        (
            predicate: ah => ah.EducationAccount != null && ah.EducationAccount.IsActive,
            include: query => query.Include(ah => ah.EducationAccount),
            cancellationToken: cancellationToken
        );

        foreach (var holder in accountHolders)
        {
            if (holder.IsEligibleForAccountClosure(_options.AgeThreshold, today))
            {
                holder.EducationAccount!.CloseAccount();
            }
        }

        await _unitOfWork.SaveAsync();
    }
}