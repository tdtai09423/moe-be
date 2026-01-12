using System;
using System.Collections.Generic;

namespace MOE_System.Domain.Entities;

public partial class Provider
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
}
