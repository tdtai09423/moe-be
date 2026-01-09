using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MOE_System.EService.Infrastructure.Data.Entities;

[Index("CourseOfferingId", Name = "IX_Enrollments_CourseOfferingId")]
[Index("EducationAccountId", Name = "IX_Enrollments_EducationAccountId")]
public partial class Enrollment
{
    [Key]
    public string Id { get; set; } = null!;

    public string CourseOfferingId { get; set; } = null!;

    public string EducationAccountId { get; set; } = null!;

    public DateTime EnrollDate { get; set; }

    [StringLength(50)]
    public string Status { get; set; } = null!;

    [ForeignKey("CourseOfferingId")]
    [InverseProperty("Enrollments")]
    public virtual CourseOffering CourseOffering { get; set; } = null!;

    [ForeignKey("EducationAccountId")]
    [InverseProperty("Enrollments")]
    public virtual EducationAccount EducationAccount { get; set; } = null!;

    [InverseProperty("Enrollment")]
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
