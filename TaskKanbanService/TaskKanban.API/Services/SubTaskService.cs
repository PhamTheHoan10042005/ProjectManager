using Microsoft.EntityFrameworkCore;
using TaskKanban.API.Data;
using TaskKanban.API.DTOs;
using TaskKanban.API.Models;

namespace TaskKanban.API.Services;

public interface ISubTaskService
{
    Task<IReadOnlyList<SubTaskResponse>> GetByTaskAsync(Guid taskId, CancellationToken cancellationToken = default);
    Task<SubTaskResponse?> CreateAsync(Guid taskId, CreateSubTaskRequest request, CancellationToken cancellationToken = default);
    Task<SubTaskResponse?> UpdateAsync(Guid taskId, Guid subTaskId, UpdateSubTaskRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid taskId, Guid subTaskId, CancellationToken cancellationToken = default);
}

public class SubTaskService(TaskDbContext dbContext) : ISubTaskService
{
    public async Task<IReadOnlyList<SubTaskResponse>> GetByTaskAsync(
        Guid taskId,
        CancellationToken cancellationToken = default)
    {
        var subTasks = await dbContext.SubTasks
            .AsNoTracking()
            .Where(s => s.TaskId == taskId)
            .OrderBy(s => s.OrderIndex)
            .ToListAsync(cancellationToken);

        return subTasks.Select(TaskMapper.ToResponse).ToList();
    }

    public async Task<SubTaskResponse?> CreateAsync(
        Guid taskId,
        CreateSubTaskRequest request,
        CancellationToken cancellationToken = default)
    {
        var taskExists = await dbContext.Tasks.AnyAsync(t => t.Id == taskId, cancellationToken);
        if (!taskExists) return null;

        var maxOrder = await dbContext.SubTasks
            .Where(s => s.TaskId == taskId)
            .Select(s => (int?)s.OrderIndex)
            .MaxAsync(cancellationToken) ?? -1;

        var subTask = new SubTask
        {
            Id = Guid.NewGuid(),
            TaskId = taskId,
            Title = request.Title,
            OrderIndex = maxOrder + 1
        };

        dbContext.SubTasks.Add(subTask);
        await dbContext.SaveChangesAsync(cancellationToken);

        return TaskMapper.ToResponse(subTask);
    }

    public async Task<SubTaskResponse?> UpdateAsync(
        Guid taskId,
        Guid subTaskId,
        UpdateSubTaskRequest request,
        CancellationToken cancellationToken = default)
    {
        var subTask = await dbContext.SubTasks
            .FirstOrDefaultAsync(s => s.Id == subTaskId && s.TaskId == taskId, cancellationToken);

        if (subTask is null) return null;

        subTask.Title = request.Title;
        subTask.IsCompleted = request.IsCompleted;

        await dbContext.SaveChangesAsync(cancellationToken);
        return TaskMapper.ToResponse(subTask);
    }

    public async Task<bool> DeleteAsync(
        Guid taskId,
        Guid subTaskId,
        CancellationToken cancellationToken = default)
    {
        var subTask = await dbContext.SubTasks
            .FirstOrDefaultAsync(s => s.Id == subTaskId && s.TaskId == taskId, cancellationToken);

        if (subTask is null) return false;

        dbContext.SubTasks.Remove(subTask);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
