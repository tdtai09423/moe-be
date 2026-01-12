using System;
using System.Collections.Generic;

namespace MOE_System.Domain.Entities;

public partial class HistoryOfChange
{
    public string Id { get; set; } = null!;

    public string EducationAccountId { get; set; } = null!;

    public decimal Amount { get; set; }

    public string Type { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual EducationAccount EducationAccount { get; set; } = null!;
}
