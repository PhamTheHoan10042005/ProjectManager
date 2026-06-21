namespace TaskKanban.API.Models;

public class TaskItem
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? SprintId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public string? LabelColor { get; set; }
    public DateTime? Deadline { get; set; }
    public KanbanStatus Status { get; set; } = KanbanStatus.Backlog;
    public Guid? AssigneeId { get; set; }
    public int OrderIndex { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<SubTask> SubTasks { get; set; } = [];
    public ICollection<TimeLog> TimeLogs { get; set; } = [];
}
