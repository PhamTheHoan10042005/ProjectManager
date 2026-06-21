using System.Text.Json.Serialization;

namespace CommentNotifyService.DTOs
{
    public class CreateCommentDto
    {
        public Guid TaskId { get; set; }
        public string Content { get; set; } = string.Empty;
    }

    public class AddCommentDto
    {
        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;
    }
}
