using System;
using System.Collections.Generic;

namespace MOE_System.Domain.Entities;

public partial class TopupRule
{
    public string Id { get; set; } = null!;

    public string RuleName { get; set; } = null!;

    public string AgeCondition { get; set; } = null!;

    public string BalanceCondition { get; set; } = null!;

    public string EduLevelCond { get; set; } = null!;

    public decimal TopupAmount { get; set; }

    public virtual ICollection<BatchRuleExecution> BatchRuleExecutions { get; set; } = new List<BatchRuleExecution>();
}
