using System.ComponentModel.DataAnnotations;

namespace CommentNotifyService.Models
{
    public class ActivityLog
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid ProjectId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        public string? UserName { get; set; }

        [Required]
        public string Action { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
