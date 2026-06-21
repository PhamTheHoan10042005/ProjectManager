using Microsoft.EntityFrameworkCore;
using TaskKanban.API.Data;
using TaskKanban.API.DTOs;
using TaskKanban.API.Models;

namespace TaskKanban.API.Services;

public interface ITimeLogService
{
    Task<TaskTimeSummaryResponse?> GetByTaskAsync(Guid taskId, CancellationToken cancellationToken = default);
    Task<TimeLogResponse?> CreateAsync(Guid taskId, CreateTimeLogRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid taskId, Guid timeLogId, CancellationToken cancellationToken = default);
}

public class TimeLogService(TaskDbContext dbContext) : ITimeLogService
{
    public async Task<TaskTimeSummaryResponse?> GetByTaskAsync(
        Guid taskId,
        CancellationToken cancellationToken = default)
    {
        var taskExists = await dbContext.Tasks.AnyAsync(t => t.Id == taskId, cancellationToken);
        if (!taskExists) return null;

        var logs = await dbContext.TimeLogs
            .AsNoTracking()
            .Where(l => l.TaskId == taskId)
            .OrderByDescending(l => l.LoggedDate)
            .ToListAsync(cancellationToken);

        var responses = logs.Select(TaskMapper.ToResponse).ToList();
        return new TaskTimeSummaryResponse(taskId, responses.Sum(l => l.Hours), responses);
    }

    public async Task<TimeLogResponse?> CreateAsync(
        Guid taskId,
        CreateTimeLogRequest request,
        CancellationToken cancellationToken = default)
    {
        var taskExists = await dbContext.Tasks.AnyAsync(t => t.Id == taskId, cancellationToken);
        if (!taskExists) return null;

        var timeLog = new TimeLog
        {
            Id = Guid.NewGuid(),
            TaskId = taskId,
            UserId = request.UserId,
            Hours = request.Hours,
            Description = request.Description,
            LoggedDate = request.LoggedDate.Date
        };

        dbContext.TimeLogs.Add(timeLog);
        await dbContext.SaveChangesAsync(cancellationToken);

        return TaskMapper.ToResponse(timeLog);
    }

    public async Task<bool> DeleteAsync(
        Guid taskId,
        Guid timeLogId,
        CancellationToken cancellationToken = default)
    {
        var timeLog = await dbContext.TimeLogs
            .FirstOrDefaultAsync(l => l.Id == timeLogId && l.TaskId == taskId, cancellationToken);

        if (timeLog is null) return false;

        dbContext.TimeLogs.Remove(timeLog);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
