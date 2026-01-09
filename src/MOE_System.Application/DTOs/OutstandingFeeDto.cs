namespace MOE_System.Application.DTOs;

public class OutstandingFeeDto
{
    public string AccountId { get; set; } = string.Empty;
    public decimal TotalOutstandingAmount { get; set; }
    public List<InvoiceDetailDto> OutstandingInvoices { get; set; } = new();
}

public class InvoiceDetailDto
{
    public string InvoiceId { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime DueDate { get; set; }
    public string Status { get; set; } = string.Empty;
}
