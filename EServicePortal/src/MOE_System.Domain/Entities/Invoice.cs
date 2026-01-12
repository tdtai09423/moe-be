using System;
using System.Collections.Generic;

namespace MOE_System.Domain.Entities;

public partial class Invoice
{
    public string Id { get; set; } = null!;

    public string EnrollmentId { get; set; } = null!;

    public decimal Amount { get; set; }

    public DateTime DueDate { get; set; }

    public string Status { get; set; } = null!;

    public virtual Enrollment Enrollment { get; set; } = null!;

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
