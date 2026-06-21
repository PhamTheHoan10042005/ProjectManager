using System;
using System.Collections.Generic;

namespace Project.Infrastructure;

public partial class Permission
{
    public int PermissionId { get; set; }

    public string? Name { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
