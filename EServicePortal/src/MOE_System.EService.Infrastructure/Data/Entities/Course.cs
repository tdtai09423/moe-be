using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MOE_System.EService.Infrastructure.Data.Entities;

[Index("ProviderId", Name = "IX_Courses_ProviderId")]
public partial class Course
{
    [Key]
    public string Id { get; set; } = null!;

    [StringLength(200)]
    public string CourseName { get; set; } = null!;

    [StringLength(50)]
    public string CourseCode { get; set; } = null!;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal FeeAmount { get; set; }

    public int DurationByMonth { get; set; }

    public string ProviderId { get; set; } = null!;

    [StringLength(50)]
    public string PaymentType { get; set; } = null!;

    public string? BillingCycle { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? DeletedAt { get; set; }

    public string? DeletedBy { get; set; }

    [InverseProperty("Course")]
    public virtual ICollection<CourseOffering> CourseOfferings { get; set; } = new List<CourseOffering>();

    [ForeignKey("ProviderId")]
    [InverseProperty("Courses")]
    public virtual Provider Provider { get; set; } = null!;
}
