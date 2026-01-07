using MOE_System.Domain.Common;

namespace MOE_System.Domain.Entities;

public class Course : BaseEntity
{
    public string CourseName { get; set; } = string.Empty;
    public string CourseCode { get; set; } = string.Empty;
    public decimal FeeAmount { get; set; }
    public int DurationByMonth { get; set; }
    public string ProviderId { get; set; } = string.Empty;
    public string PaymentType { get; set; } = string.Empty;
    public string? BillingCycle { get; set; }

    // Navigation properties
    public Provider? Provider { get; set; }
    public ICollection<CourseOffering> CourseOfferings { get; set; } = new List<CourseOffering>();
}
