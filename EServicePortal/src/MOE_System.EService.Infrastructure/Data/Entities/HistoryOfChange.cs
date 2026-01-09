using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MOE_System.EService.Infrastructure.Data.Entities;

[Index("EducationAccountId", Name = "IX_HistoryOfChanges_EducationAccountId")]
public partial class HistoryOfChange
{
    [Key]
    public string Id { get; set; } = null!;

    public string EducationAccountId { get; set; } = null!;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Amount { get; set; }

    [StringLength(50)]
    public string Type { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    [ForeignKey("EducationAccountId")]
    [InverseProperty("HistoryOfChanges")]
    public virtual EducationAccount EducationAccount { get; set; } = null!;
}
