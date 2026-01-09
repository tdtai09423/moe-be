using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MOE_System.Domain.Entities;

public partial class TopupRule
{
    [Key]
    public string Id { get; set; } = null!;

    [StringLength(200)]
    public string RuleName { get; set; } = null!;

    public string AgeCondition { get; set; } = null!;

    public string BalanceCondition { get; set; } = null!;

    public string EduLevelCond { get; set; } = null!;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal TopupAmount { get; set; }

    [InverseProperty("Rule")]
    public virtual ICollection<BatchRuleExecution> BatchRuleExecutions { get; set; } = new List<BatchRuleExecution>();
}
