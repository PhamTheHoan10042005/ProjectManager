namespace TaskKanban.API.Models;

public class TimeLog
{
    public Guid Id { get; set; }
    public Guid TaskId { get; set; }
    public Guid UserId { get; set; }
    public decimal Hours { get; set; }
    public string? Description { get; set; }
    public DateTime LoggedDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public TaskItem Task { get; set; } = null!;
}
