namespace MOE_System.Application.Common;

public sealed class AccountClosureOptions
{
    public bool Enabled { get; set; }
    public int AgeThreshold { get; set; }
    public int ProcessingDay { get; set; }
    public int ProcessingMonth { get; set; }
    public string TimeZone { get; set; } = string.Empty;
}