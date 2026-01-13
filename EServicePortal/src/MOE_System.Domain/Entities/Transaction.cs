namespace MOE_System.Domain.Entities;

public class Transaction
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public decimal Amount { get; set; }
    public string InvoiceId { get; set; } = string.Empty;
    public DateTime TransactionAt { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal BalanceBefore { get; set; }
    public decimal BalanceAfter { get; set; }

    // Navigation property
    public Invoice? Invoice { get; set; }
}
