using System;
using System.Collections.Generic;

namespace MOE_System.Domain.Entities;

public partial class EducationAccount
{
    public string Id { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public DateTime? LastLoginAt { get; set; }

    public DateTime? ExpiredAt { get; set; }

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

    public virtual AccountHolder AccountHolder { get; set; } = null!;

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    public virtual ICollection<HistoryOfChange> HistoryOfChanges { get; set; } = new List<HistoryOfChange>();
}
