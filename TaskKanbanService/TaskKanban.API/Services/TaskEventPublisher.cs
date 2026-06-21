using MassTransit;
using TaskKanban.API.Events;

namespace TaskKanban.API.Services;

public interface ITaskEventPublisher
{
    Task PublishStatusChangedAsync(TaskStatusChangedEvent @event, CancellationToken cancellationToken = default);
    Task PublishAssignedAsync(TaskAssignedEvent @event, CancellationToken cancellationToken = default);
}

public class TaskEventPublisher(IPublishEndpoint publishEndpoint) : ITaskEventPublisher
{
    public Task PublishStatusChangedAsync(TaskStatusChangedEvent @event, CancellationToken cancellationToken = default)
        => publishEndpoint.Publish(@event, cancellationToken);

    public Task PublishAssignedAsync(TaskAssignedEvent @event, CancellationToken cancellationToken = default)
        => publishEndpoint.Publish(@event, cancellationToken);
}
