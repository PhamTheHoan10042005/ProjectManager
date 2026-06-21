using System.ComponentModel.DataAnnotations;

namespace CommentNotifyService.Models
{
    public class Notification
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserId { get; set; } // ID của người nhận thông báo (từ bảng Users)

        [Required]
        public string Title { get; set; } = string.Empty; // Tiêu đề thông báo

        [Required]
        public string Message { get; set; } = string.Empty; // Nội dung chi tiết thông báo

        public bool IsRead { get; set; } = false; // Trạng thái: false = Chưa đọc, true = Đã đọc

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Thời gian tạo thông báo
    }
}   