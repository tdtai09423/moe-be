using System;
using System.Collections.Generic;

namespace MOE_System.Domain.Entities;

public partial class BatchRuleExecution
{
    public string Id { get; set; } = null!;

    public string BatchId { get; set; } = null!;

    public string RuleId { get; set; } = null!;

    public virtual BatchExecution Batch { get; set; } = null!;

    public virtual TopupRule Rule { get; set; } = null!;
}
