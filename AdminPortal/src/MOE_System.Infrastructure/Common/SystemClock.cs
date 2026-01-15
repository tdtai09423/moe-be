using MOE_System.Application.Common.Interfaces;

namespace MOE_System.Infrastructure.Common;

public class SystemClock : IClock
{
    public DateOnly TodayInTimeZone(string timeZone)
    {
        var tzInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
        var currentTime = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, tzInfo);
        return DateOnly.FromDateTime(currentTime.DateTime);
    }

    public DateTime UtcNow() => DateTime.UtcNow;
}