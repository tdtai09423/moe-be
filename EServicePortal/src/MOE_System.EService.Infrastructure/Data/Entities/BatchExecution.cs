using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MOE_System.EService.Infrastructure.Data.Entities;

public partial class BatchExecution
{
    [Key]
    public string Id { get; set; } = null!;

    public DateTime ScheduledTime { get; set; }

    public DateTime? ExecutedTime { get; set; }

    [StringLength(50)]
    public string Status { get; set; } = null!;

    [InverseProperty("Batch")]
    public virtual ICollection<BatchRuleExecution> BatchRuleExecutions { get; set; } = new List<BatchRuleExecution>();
}
