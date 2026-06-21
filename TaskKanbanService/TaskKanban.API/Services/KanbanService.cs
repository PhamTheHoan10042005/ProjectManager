using Microsoft.EntityFrameworkCore;
using TaskKanban.API.Data;
using TaskKanban.API.DTOs;
using TaskKanban.API.Models;

namespace TaskKanban.API.Services;

public interface IKanbanService
{
    Task<KanbanBoardResponse> GetBoardAsync(Guid projectId, Guid? sprintId, CancellationToken cancellationToken = default);
}

public class KanbanService(TaskDbContext dbContext) : IKanbanService
{
    private static readonly KanbanStatus[] ColumnOrder =
    [
        KanbanStatus.Backlog,
        KanbanStatus.ToDo,
        KanbanStatus.InProgress,
        KanbanStatus.Review,
        KanbanStatus.Done
    ];

    public async Task<KanbanBoardResponse> GetBoardAsync(
        Guid projectId,
        Guid? sprintId,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.Tasks
            .AsNoTracking()
            .Include(t => t.SubTasks)
            .Include(t => t.TimeLogs)
            .Where(t => t.ProjectId == projectId);

        if (sprintId.HasValue)
            query = query.Where(t => t.SprintId == sprintId);

        var tasks = await query.ToListAsync(cancellationToken);

        var columns = ColumnOrder.Select(status => new KanbanColumnResponse(
            status,
            KanbanColumnNames.GetName(status),
            tasks
                .Where(t => t.Status == status)
                .OrderBy(t => t.OrderIndex)
                .Select(TaskMapper.ToResponse)
                .ToList())).ToList();

        return new KanbanBoardResponse(projectId, sprintId, columns);
    }
}
