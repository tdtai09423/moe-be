using System;
using System.Collections.Generic;

namespace MOE_System.Domain.Entities;

public partial class Transaction
{
    public string Id { get; set; } = null!;

    public decimal Amount { get; set; }

    public string InvoiceId { get; set; } = null!;

    public DateTime TransactionAt { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public string Status { get; set; } = null!;

    public decimal BalanceBefore { get; set; }

    public decimal BalanceAfter { get; set; }

    public virtual Invoice Invoice { get; set; } = null!;
}
