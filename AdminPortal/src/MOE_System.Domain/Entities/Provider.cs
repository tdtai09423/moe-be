namespace MOE_System.Domain.Entities;

public class Provider
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;

    // Navigation property
    public ICollection<Course> Courses { get; set; } = new List<Course>();
}
