namespace MOE_System.Domain.Entities;

public class Enrollment
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string CourseOfferingId { get; set; } = string.Empty;
    public string EducationAccountId { get; set; } = string.Empty;
    public DateTime EnrollDate { get; set; }
    public string Status { get; set; } = string.Empty;

    // Navigation properties
    public CourseOffering? CourseOffering { get; set; }
    public EducationAccount? EducationAccount { get; set; }
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
