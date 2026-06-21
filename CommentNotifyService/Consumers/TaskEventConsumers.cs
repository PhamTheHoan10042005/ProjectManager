using CommentNotifyService.Data;
using CommentNotifyService.Models;
using MassTransit;
using TaskKanban.API.Events;

namespace CommentNotifyService.Consumers;

public class TaskStatusChangedConsumer(IServiceScopeFactory scopeFactory) : IConsumer<TaskStatusChangedEvent>
{
    public async Task Consume(ConsumeContext<TaskStatusChangedEvent> consumeContext)
    {
        var evt = consumeContext.Message;
        var assigneeId = evt.ChangedByUserId ?? Guid.Empty;
        if (assigneeId == Guid.Empty) return;

        using var scope = scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        context.Notifications.Add(new Notification
        {
            UserId = assigneeId,
            Title = "Cập nhật trạng thái task",
            Message = $"Task đã chuyển từ trạng thái {evt.OldStatus} sang {evt.NewStatus}.",
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        });

        context.ActivityLogs.Add(new ActivityLog
        {
            ProjectId = evt.ProjectId,
            UserId = assigneeId,
            Action = "task_status_changed",
            Description = $"Task {evt.TaskId}: {evt.OldStatus} → {evt.NewStatus}"
        });

        await context.SaveChangesAsync();
    }
}

public class TaskAssignedConsumer(IServiceScopeFactory scopeFactory) : IConsumer<TaskAssignedEvent>
{
    public async Task Consume(ConsumeContext<TaskAssignedEvent> consumeContext)
    {
        var evt = consumeContext.Message;

        using var scope = scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        context.Notifications.Add(new Notification
        {
            UserId = evt.NewAssigneeId,
            Title = "Được giao task mới",
            Message = $"Bạn được giao task: {evt.TaskTitle}",
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        });

        context.ActivityLogs.Add(new ActivityLog
        {
            ProjectId = evt.ProjectId,
            UserId = evt.NewAssigneeId,
            Action = "task_assigned",
            Description = $"Được giao task: {evt.TaskTitle}"
        });

        await context.SaveChangesAsync();
    }
}
