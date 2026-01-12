using System;
using System.Collections.Generic;

namespace MOE_System.Domain.Entities;

public partial class Admin
{
    public string Id { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;
}
