using System;

namespace Project.Infrastructure.Entities;

public partial class Sprint
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public string Name { get; set; } = null!;
    public string? Goal { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = null!;
}
