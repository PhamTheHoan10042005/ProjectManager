using System;

namespace Project.Infrastructure.Entities;

public partial class ProjectMember
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public string UserId { get; set; } = null!;
    public DateTime? JoinedAt { get; set; }
    public int Role { get; set; }
}
