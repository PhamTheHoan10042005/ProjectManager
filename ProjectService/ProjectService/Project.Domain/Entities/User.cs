using System;
using System.Collections.Generic;

namespace Project.Infrastructure;

public partial class User
{
    public int UserId { get; set; }

    public string? Name { get; set; }

    public string? Address { get; set; }

    public bool? Gender { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public bool? IsDeleted { get; set; }

    public int? PermissionId { get; set; }

    public virtual Permission? Permission { get; set; }
}
