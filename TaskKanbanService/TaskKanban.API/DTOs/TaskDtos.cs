using System.ComponentModel.DataAnnotations;
using TaskKanban.API.Models;

namespace TaskKanban.API.DTOs;

public record TaskResponse(
    Guid Id,
    Guid ProjectId,
    Guid? SprintId,
    string Title,
    string? Description,
    TaskPriority Priority,
    string? LabelColor,
    DateTime? Deadline,
    KanbanStatus Status,
    Guid? AssigneeId,
    int OrderIndex,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    int SubTaskCount,
    decimal TotalLoggedHours);

public class CreateTaskRequest
{
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(4000)]
    public string? Description { get; set; }

    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public string? LabelColor { get; set; }
    public DateTime? Deadline { get; set; }
    public KanbanStatus Status { get; set; } = KanbanStatus.Backlog;
    public Guid? SprintId { get; set; }
    public Guid? AssigneeId { get; set; }
}

public class UpdateTaskRequest
{
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(4000)]
    public string? Description { get; set; }

    public TaskPriority Priority { get; set; }
    public string? LabelColor { get; set; }
    public DateTime? Deadline { get; set; }
    public Guid? SprintId { get; set; }
}

public class AssignTaskRequest
{
    [Required]
    public Guid AssigneeId { get; set; }
}

public class UpdateTaskStatusRequest
{
    [Required]
    public KanbanStatus Status { get; set; }

    public int? OrderIndex { get; set; }
}

public class MoveTaskRequest
{
    [Required]
    public KanbanStatus Status { get; set; }

    [Required]
    public int OrderIndex { get; set; }
}
