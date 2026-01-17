using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
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

    public async Task CloseEducationAccountManuallyAsync(string nric, CancellationToken cancellationToken)
    {
        var accountHolderRepository = _unitOfWork.GetRepository<AccountHolder>();

        var holder = await accountHolderRepository.FirstOrDefaultAsync(
            predicate: ah => ah.NRIC == nric,
            include: query => query.Include(ah => ah.EducationAccount),
            asTracking: true,
            cancellationToken: cancellationToken
        );

        if (holder == null || holder.EducationAccount == null)
        {
            throw new NotFoundException("Education account holder not found.");
        }

        holder.EducationAccount.CloseAccount();

        await _unitOfWork.SaveAsync();
    }

    public async Task AutoCloseEducationAccountsAsync(CancellationToken cancellationToken)
    {
        if (!_options.Enabled)
        {
            return;
        }

        var today = _clock.TodayInTimeZone(_options.TimeZone);

        var scheduledDate = new DateOnly(today.Year, _options.ProcessingMonth, _options.ProcessingDay);

        if (today < scheduledDate)
        {
            return;
        }

        var accountHolderRepository = _unitOfWork.GetRepository<AccountHolder>();

        var holders = await accountHolderRepository.ToListAsync(
            predicate: ah => ah.EducationAccount != null && ah.EducationAccount.IsActive && ah.EducationAccount.ClosedDate == null,
            include: query => query.Include(ah => ah.EducationAccount),
            asTracking: true,
            cancellationToken: cancellationToken
        );

        foreach (var holder in holders)
        {
            if (!holder.IsEligibleForAccountClosure(_options.AgeThreshold, today.Year))
                continue;

            holder.EducationAccount!.CloseAccount();
        }

        await _unitOfWork.SaveAsync();
    }
}