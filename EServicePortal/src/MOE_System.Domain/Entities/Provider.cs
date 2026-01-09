using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MOE_System.Domain.Entities;

public partial class Provider
{
    [Key]
    public string Id { get; set; } = null!;

    [StringLength(200)]
    public string Name { get; set; } = null!;

    [InverseProperty("Provider")]
    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
}
