namespace MOE_System.Application.Common.Dashboard;

public static class DateRangeResolver
{
    public static DateRange? Resolve(
        DateRangeType dateRangeType,
        DateTime nowUtc,
        DateOnly? from = null,
        DateOnly? to = null
    )
    {
        var today = nowUtc.Date;
        var monthStart = new DateTime(today.Year, today.Month, 1);

        return dateRangeType switch
        {
            DateRangeType.ThisMonth => 
            new DateRange(
                StartDate: monthStart,
                EndDate: nowUtc
            ),

            DateRangeType.LastMonth => 
            new DateRange(
                StartDate: monthStart.AddMonths(-1),
                EndDate: monthStart
            ),

            DateRangeType.Last30Days => new DateRange(
                StartDate: nowUtc.AddDays(-30),
                EndDate: nowUtc
            ),

            DateRangeType.Last3Months => new DateRange(
                StartDate: nowUtc.AddMonths(-3),
                EndDate: nowUtc
            ),

            DateRangeType.Last6Months => new DateRange(
                StartDate: nowUtc.AddMonths(-6),
                EndDate: nowUtc
            ),

            DateRangeType.AllTime => null,

            DateRangeType.Custom
                when from.HasValue && to.HasValue => 
                new DateRange(
                    StartDate: from.Value.ToDateTime(TimeOnly.MinValue),
                    EndDate: to.Value.AddDays(1).ToDateTime(TimeOnly.MinValue)
                ),

            _ => throw new ArgumentException("Invalid date range type or missing from/to dates for custom range.")
        };
    }
}