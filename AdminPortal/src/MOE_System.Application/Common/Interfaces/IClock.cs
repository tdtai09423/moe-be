namespace MOE_System.Application.Common.Interfaces;

public interface IClock
{
    DateOnly TodayInTimeZone(string timeZone);
    DateTime UtcNow();
}