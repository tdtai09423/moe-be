namespace MOE_System.Domain.Entities;

public class HistoryOfChange
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string EducationAccountId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Type { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public EducationAccount? EducationAccount { get; set; }
}
