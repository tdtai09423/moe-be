using FluentValidation;
using MOE_System.Application.DTOs.Dashboard.Request;
using MOE_System.Application.Common.Dashboard;

namespace MOE_System.Application.Validators.Dashboard;

public sealed class DashboardOverviewRequestValidator : AbstractValidator<DashboardOverviewRequest>
{
    public DashboardOverviewRequestValidator()
    {
        RuleFor(x => x.DateRangeType)
            .IsInEnum()
            .WithMessage("Invalid DateRangeType value.");

        RuleFor(x => x.From)
            .NotNull()
            .When(x => x.DateRangeType == DateRangeType.Custom)
            .WithMessage("'From' date must be provided for custom date range.");

        RuleFor(x => x.To)
            .NotNull()
            .When(x => x.DateRangeType == DateRangeType.Custom)
            .WithMessage("'To' date must be provided for custom date range.");
    }
}