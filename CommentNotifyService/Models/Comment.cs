using System.ComponentModel.DataAnnotations;

namespace CommentNotifyService.Models
{
    public class Comment
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid TaskId { get; set; } // Reference ID sang service của Nhóm 2

        [Required]
        public Guid UserId { get; set; } // ID của người dùng hệ thống (từ bảng Users)

        [Required]
        public string Content { get; set; } = string.Empty; // Nội dung bình luận

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Thời gian tạo
    }
}