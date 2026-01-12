using System;
using System.Collections.Generic;

namespace MOE_System.Domain.Entities;

public partial class Course
{
    public string Id { get; set; } = null!;

    public string CourseName { get; set; } = null!;

    public string CourseCode { get; set; } = null!;

    public decimal FeeAmount { get; set; }

    public int DurationByMonth { get; set; }

    public string ProviderId { get; set; } = null!;

    public string PaymentType { get; set; } = null!;

    public string? BillingCycle { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? DeletedAt { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime EndDate { get; set; }

    public DateTime StartDate { get; set; }

    public string Status { get; set; } = null!;

    public string TermName { get; set; } = null!;

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    public virtual Provider Provider { get; set; } = null!;
}
