using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MOE_System.EService.Infrastructure.Data.Entities;

[Index("CourseId", Name = "IX_CourseOfferings_CourseID")]
public partial class CourseOffering
{
    [Key]
    public string Id { get; set; } = null!;

    [Column("CourseID")]
    public string CourseId { get; set; } = null!;

    [StringLength(100)]
    public string TermName { get; set; } = null!;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    [StringLength(50)]
    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? DeletedAt { get; set; }

    public string? DeletedBy { get; set; }

    [ForeignKey("CourseId")]
    [InverseProperty("CourseOfferings")]
    public virtual Course Course { get; set; } = null!;

    [InverseProperty("CourseOffering")]
    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
