using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MOE_System.Domain.Entities;

[Index("EnrollmentId", Name = "IX_Invoices_EnrollmentID")]
public partial class Invoice
{
    [Key]
    public string Id { get; set; } = null!;

    [Column("EnrollmentID")]
    public string EnrollmentId { get; set; } = null!;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Amount { get; set; }

    public DateTime DueDate { get; set; }

    [StringLength(50)]
    public string Status { get; set; } = null!;

    [ForeignKey("EnrollmentId")]
    [InverseProperty("Invoices")]
    public virtual Enrollment Enrollment { get; set; } = null!;

    [InverseProperty("Invoice")]
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
