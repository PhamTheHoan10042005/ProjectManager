using TaskKanban.API.Models;

namespace TaskKanban.API.DTOs;

public record KanbanColumnResponse(KanbanStatus Status, string Name, IReadOnlyList<TaskResponse> Tasks);

public record KanbanBoardResponse(Guid ProjectId, Guid? SprintId, IReadOnlyList<KanbanColumnResponse> Columns);

public static class KanbanColumnNames
{
    public static string GetName(KanbanStatus status) => status switch
    {
        KanbanStatus.Backlog => "Backlog",
        KanbanStatus.ToDo => "To Do",
        KanbanStatus.InProgress => "In Progress",
        KanbanStatus.Review => "Review",
        KanbanStatus.Done => "Done",
        _ => status.ToString()
    };
}
