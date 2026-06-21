using System.Text.Json;

namespace Project.Infrastructure.Messaging
{
    public static class EventPublisher
    {
        public static Task PublishEventAsync(string eventName, object payload)
        {
            // Đây chỉ là mô phỏng - trong thực tế bạn sẽ tích hợp với Kafka, RabbitMQ,...
            var message = JsonSerializer.Serialize(new { eventName, payload, publishedAt = System.DateTime.UtcNow });
            System.Diagnostics.Debug.WriteLine("Published event: " + message);
            return Task.CompletedTask;
        }
    }
}
