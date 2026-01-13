using System;
using System.Collections.Generic;

namespace MOE_System.Domain.Entities;

public partial class Enrollment
{
    public string Id { get; set; } = null!;

    public string CourseId { get; set; } = null!;

    public string EducationAccountId { get; set; } = null!;

    public DateTime EnrollDate { get; set; }

    public string Status { get; set; } = null!;

    public virtual Course Course { get; set; } = null!;

    public virtual EducationAccount EducationAccount { get; set; } = null!;

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
