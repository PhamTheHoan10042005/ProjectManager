using TaskKanban.API.Models;

namespace TaskKanban.API.Events;

public record TaskStatusChangedEvent
{
    public Guid TaskId { get; init; }
    public Guid ProjectId { get; init; }
    public KanbanStatus OldStatus { get; init; }
    public KanbanStatus NewStatus { get; init; }
    public Guid? ChangedByUserId { get; init; }
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
}

public record TaskAssignedEvent
{
    public Guid TaskId { get; init; }
    public Guid ProjectId { get; init; }
    public string TaskTitle { get; init; } = string.Empty;
    public Guid? PreviousAssigneeId { get; init; }
    public Guid NewAssigneeId { get; init; }
    public Guid? AssignedByUserId { get; init; }
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
}
