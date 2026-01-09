using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MOE_System.Domain.Entities;

[Index("AccountHolderId", Name = "IX_EducationAccounts_AccountHolderId", IsUnique = true)]
public partial class EducationAccount
{
    [Key]
    public string Id { get; set; } = null!;

    [StringLength(100)]
    public string UserName { get; set; } = null!;

    [StringLength(256)]
    public string Password { get; set; } = null!;

    public DateTime? LastLoginAt { get; set; }

    public DateTime? ExpiredAt { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Balance { get; set; }

    public bool IsActive { get; set; }

    public DateTime? ClosedDate { get; set; }

    public string AccountHolderId { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? DeletedAt { get; set; }

    public string? DeletedBy { get; set; }

    [ForeignKey("AccountHolderId")]
    [InverseProperty("EducationAccount")]
    public virtual AccountHolder AccountHolder { get; set; } = null!;

    [InverseProperty("EducationAccount")]
    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    [InverseProperty("EducationAccount")]
    public virtual ICollection<HistoryOfChange> HistoryOfChanges { get; set; } = new List<HistoryOfChange>();
}
