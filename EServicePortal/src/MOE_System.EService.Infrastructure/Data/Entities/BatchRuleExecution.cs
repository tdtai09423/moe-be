using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MOE_System.EService.Infrastructure.Data.Entities;

[Index("BatchId", Name = "IX_BatchRuleExecutions_BatchID")]
[Index("RuleId", Name = "IX_BatchRuleExecutions_RuleID")]
public partial class BatchRuleExecution
{
    [Key]
    public string Id { get; set; } = null!;

    [Column("BatchID")]
    public string BatchId { get; set; } = null!;

    [Column("RuleID")]
    public string RuleId { get; set; } = null!;

    [ForeignKey("BatchId")]
    [InverseProperty("BatchRuleExecutions")]
    public virtual BatchExecution Batch { get; set; } = null!;

    [ForeignKey("RuleId")]
    [InverseProperty("BatchRuleExecutions")]
    public virtual TopupRule Rule { get; set; } = null!;
}
