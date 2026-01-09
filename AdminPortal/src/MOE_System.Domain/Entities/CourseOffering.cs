using MOE_System.Domain.Common;

namespace MOE_System.Domain.Entities;

public class CourseOffering : BaseEntity
{
    public string CourseID { get; set; } = string.Empty;
    public string TermName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = string.Empty;

    // Navigation properties
    public Course? Course { get; set; }
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
