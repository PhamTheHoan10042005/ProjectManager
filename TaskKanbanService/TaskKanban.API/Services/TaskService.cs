using Microsoft.EntityFrameworkCore;
using TaskKanban.API.Data;
using TaskKanban.API.DTOs;
using TaskKanban.API.Events;
using TaskKanban.API.Models;

namespace TaskKanban.API.Services;

public interface ITaskService
{
    Task<IReadOnlyList<TaskResponse>> GetByProjectAsync(Guid projectId, Guid? sprintId, CancellationToken cancellationToken = default);
    Task<TaskResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TaskResponse> CreateAsync(Guid projectId, CreateTaskRequest request, CancellationToken cancellationToken = default);
    Task<TaskResponse?> UpdateAsync(Guid id, UpdateTaskRequest request, CancellationToken cancellationToken = default);
    Task<TaskResponse?> AssignAsync(Guid id, AssignTaskRequest request, Guid? assignedByUserId, CancellationToken cancellationToken = default);
    Task<TaskResponse?> UpdateStatusAsync(Guid id, UpdateTaskStatusRequest request, Guid? changedByUserId, CancellationToken cancellationToken = default);
    Task<TaskResponse?> MoveAsync(Guid id, MoveTaskRequest request, Guid? changedByUserId, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

public class TaskService(
    TaskDbContext dbContext,
    ITaskEventPublisher eventPublisher) : ITaskService
{
    public async Task<IReadOnlyList<TaskResponse>> GetByProjectAsync(
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

        var tasks = await query
            .OrderBy(t => t.Status)
            .ThenBy(t => t.OrderIndex)
            .ToListAsync(cancellationToken);

        return tasks.Select(TaskMapper.ToResponse).ToList();
    }

    public async Task<TaskResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var task = await dbContext.Tasks
            .AsNoTracking()
            .Include(t => t.SubTasks)
            .Include(t => t.TimeLogs)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        return task is null ? null : TaskMapper.ToResponse(task);
    }

    public async Task<TaskResponse> CreateAsync(
        Guid projectId,
        CreateTaskRequest request,
        CancellationToken cancellationToken = default)
    {
        var maxOrder = await dbContext.Tasks
            .Where(t => t.ProjectId == projectId && t.Status == request.Status)
            .Select(t => (int?)t.OrderIndex)
            .MaxAsync(cancellationToken) ?? -1;

        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            SprintId = request.SprintId,
            Title = request.Title,
            Description = request.Description,
            Priority = request.Priority,
            LabelColor = request.LabelColor,
            Deadline = request.Deadline,
            Status = request.Status,
            AssigneeId = request.AssigneeId,
            OrderIndex = maxOrder + 1
        };

        dbContext.Tasks.Add(task);
        await dbContext.SaveChangesAsync(cancellationToken);

        if (request.AssigneeId.HasValue)
        {
            await eventPublisher.PublishAssignedAsync(new TaskAssignedEvent
            {
                TaskId = task.Id,
                ProjectId = task.ProjectId,
                TaskTitle = task.Title,
                PreviousAssigneeId = null,
                NewAssigneeId = request.AssigneeId.Value
            }, cancellationToken);
        }

        return TaskMapper.ToResponse(task);
    }

    public async Task<TaskResponse?> UpdateAsync(
        Guid id,
        UpdateTaskRequest request,
        CancellationToken cancellationToken = default)
    {
        var task = await dbContext.Tasks.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (task is null) return null;

        task.Title = request.Title;
        task.Description = request.Description;
        task.Priority = request.Priority;
        task.LabelColor = request.LabelColor;
        task.Deadline = request.Deadline;
        task.SprintId = request.SprintId;
        task.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task<TaskResponse?> AssignAsync(
        Guid id,
        AssignTaskRequest request,
        Guid? assignedByUserId,
        CancellationToken cancellationToken = default)
    {
        var task = await dbContext.Tasks.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (task is null) return null;

        var previousAssignee = task.AssigneeId;
        task.AssigneeId = request.AssigneeId;
        task.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);

        await eventPublisher.PublishAssignedAsync(new TaskAssignedEvent
        {
            TaskId = task.Id,
            ProjectId = task.ProjectId,
            TaskTitle = task.Title,
            PreviousAssigneeId = previousAssignee,
            NewAssigneeId = request.AssigneeId,
            AssignedByUserId = assignedByUserId
        }, cancellationToken);

        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task<TaskResponse?> UpdateStatusAsync(
        Guid id,
        UpdateTaskStatusRequest request,
        Guid? changedByUserId,
        CancellationToken cancellationToken = default)
    {
        var task = await dbContext.Tasks.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (task is null) return null;

        var oldStatus = task.Status;
        if (oldStatus == request.Status && request.OrderIndex is null)
            return await GetByIdAsync(id, cancellationToken);

        if (oldStatus != request.Status)
        {
            var orderIndex = request.OrderIndex ?? await GetNextOrderIndexAsync(task.ProjectId, request.Status, cancellationToken);
            task.Status = request.Status;
            task.OrderIndex = orderIndex;
        }
        else if (request.OrderIndex.HasValue)
        {
            task.OrderIndex = request.OrderIndex.Value;
        }

        task.UpdatedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        if (oldStatus != request.Status)
        {
            await eventPublisher.PublishStatusChangedAsync(new TaskStatusChangedEvent
            {
                TaskId = task.Id,
                ProjectId = task.ProjectId,
                OldStatus = oldStatus,
                NewStatus = request.Status,
                ChangedByUserId = changedByUserId
            }, cancellationToken);
        }

        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task<TaskResponse?> MoveAsync(
        Guid id,
        MoveTaskRequest request,
        Guid? changedByUserId,
        CancellationToken cancellationToken = default)
        => await UpdateStatusAsync(id, new UpdateTaskStatusRequest
        {
            Status = request.Status,
            OrderIndex = request.OrderIndex
        }, changedByUserId, cancellationToken);

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var task = await dbContext.Tasks.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (task is null) return false;

        dbContext.Tasks.Remove(task);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task<int> GetNextOrderIndexAsync(
        Guid projectId,
        KanbanStatus status,
        CancellationToken cancellationToken)
    {
        var maxOrder = await dbContext.Tasks
            .Where(t => t.ProjectId == projectId && t.Status == status)
            .Select(t => (int?)t.OrderIndex)
            .MaxAsync(cancellationToken);

        return (maxOrder ?? -1) + 1;
    }
}
