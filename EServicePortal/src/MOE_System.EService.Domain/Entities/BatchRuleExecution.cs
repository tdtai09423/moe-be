namespace MOE_System.EService.Domain.Entities;

public class BatchRuleExecution
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string BatchID { get; set; } = string.Empty;
    public string RuleID { get; set; } = string.Empty;

    // Navigation properties
    public BatchExecution? BatchExecution { get; set; }
    public TopupRule? TopupRule { get; set; }
}
