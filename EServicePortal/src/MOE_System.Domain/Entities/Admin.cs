using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MOE_System.Domain.Entities;

public partial class Admin
{
    [Key]
    public string Id { get; set; } = null!;

    [StringLength(100)]
    public string UserName { get; set; } = null!;

    [StringLength(256)]
    public string Password { get; set; } = null!;
}
