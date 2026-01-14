namespace MOE_System.EService.Domain.Entities;

public class BatchExecution
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTime ScheduledTime { get; set; }
    public DateTime? ExecutedTime { get; set; }
    public string Status { get; set; } = string.Empty;

    // Navigation property
    public ICollection<BatchRuleExecution> BatchRuleExecutions { get; set; } = new List<BatchRuleExecution>();
}
