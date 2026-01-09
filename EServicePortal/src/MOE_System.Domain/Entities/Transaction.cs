using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MOE_System.Domain.Entities;

[Index("InvoiceId", Name = "IX_Transactions_InvoiceId")]
public partial class Transaction
{
    [Key]
    public string Id { get; set; } = null!;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Amount { get; set; }

    public string InvoiceId { get; set; } = null!;

    public DateTime TransactionAt { get; set; }

    [StringLength(50)]
    public string PaymentMethod { get; set; } = null!;

    [StringLength(50)]
    public string Status { get; set; } = null!;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal BalanceBefore { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal BalanceAfter { get; set; }

    [ForeignKey("InvoiceId")]
    [InverseProperty("Transactions")]
    public virtual Invoice Invoice { get; set; } = null!;
}
