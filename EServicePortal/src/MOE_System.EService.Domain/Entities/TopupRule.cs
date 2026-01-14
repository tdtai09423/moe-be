namespace MOE_System.EService.Domain.Entities;

public class TopupRule
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string RuleName { get; set; } = string.Empty;
    public string AgeCondition { get; set; } = string.Empty;
    public string BalanceCondition { get; set; } = string.Empty;
    public string EduLevelCond { get; set; } = string.Empty;
    public decimal TopupAmount { get; set; }
    public string RuleTargetType { get; set; } = string.Empty; // individual or batch
    public string? TargetEducationAccountId { get; set; }
    public EducationAccount? TargetEducationAccount { get; set; }

    // Navigation property
    public ICollection<BatchRuleExecution> BatchRuleExecutions { get; set; } = new List<BatchRuleExecution>();
}
