using TaskKanban.API.DTOs;
using TaskKanban.API.Models;

namespace TaskKanban.API.Services;

public static class TaskMapper
{
    public static TaskResponse ToResponse(TaskItem task) => new(
        task.Id,
        task.ProjectId,
        task.SprintId,
        task.Title,
        task.Description,
        task.Priority,
        task.LabelColor,
        task.Deadline,
        task.Status,
        task.AssigneeId,
        task.OrderIndex,
        task.CreatedAt,
        task.UpdatedAt,
        task.SubTasks?.Count ?? 0,
        task.TimeLogs?.Sum(l => l.Hours) ?? 0);

    public static SubTaskResponse ToResponse(SubTask subTask) => new(
        subTask.Id,
        subTask.TaskId,
        subTask.Title,
        subTask.IsCompleted,
        subTask.OrderIndex,
        subTask.CreatedAt);

    public static TimeLogResponse ToResponse(TimeLog log) => new(
        log.Id,
        log.TaskId,
        log.UserId,
        log.Hours,
        log.Description,
        log.LoggedDate,
        log.CreatedAt);
}
