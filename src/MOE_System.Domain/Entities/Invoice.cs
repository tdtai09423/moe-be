namespace MOE_System.Domain.Entities;

public class Invoice
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string EnrollmentID { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime DueDate { get; set; }
    public string Status { get; set; } = string.Empty;

    // Navigation properties
    public Enrollment? Enrollment { get; set; }
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
