using System;
using System.Collections.Generic;

namespace MOE_System.Domain.Entities;

public partial class BatchExecution
{
    public string Id { get; set; } = null!;

    public DateTime ScheduledTime { get; set; }

    public DateTime? ExecutedTime { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<BatchRuleExecution> BatchRuleExecutions { get; set; } = new List<BatchRuleExecution>();
}
