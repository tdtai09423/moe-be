using MOE_System.Domain.Common;

namespace MOE_System.Domain.Entities;

public class Course : BaseEntity
{
    public string CourseName { get; set; } = string.Empty;
    public string CourseCode { get; set; } = string.Empty;
    public decimal FeeAmount { get; set; }
    public decimal? FeePerCycle { get; set; }
    public int DurationByMonth { get; set; }
    public string ProviderId { get; set; } = string.Empty;
    public string PaymentType { get; set; } = string.Empty;
    public string? BillingCycle { get; set; }
    
    // Fields from CourseOffering
    public string LearningType { get; set; } = string.Empty;
    public string TermName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = string.Empty;

    // Navigation properties
    public Provider? Provider { get; set; }
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
