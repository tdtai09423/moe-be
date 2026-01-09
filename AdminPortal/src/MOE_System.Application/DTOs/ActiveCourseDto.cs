namespace MOE_System.Application.DTOs;

public class ActiveCourseDto
{
    public string CourseId { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public string CourseCode { get; set; } = string.Empty;
    public string ProviderName { get; set; } = string.Empty;
    public string TermName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime EnrollDate { get; set; }
    public string EnrollmentStatus { get; set; } = string.Empty;
}
