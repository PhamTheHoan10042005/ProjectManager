using System.ComponentModel.DataAnnotations;

namespace CommentNotifyService.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        [Required]
        // Gộp lại làm 1 thuộc tính duy nhất, mặc định ban đầu khi đăng ký sẽ là "Viewer" hoặc "Member"
        public string Role { get; set; } = "Viewer"; // Các giá trị hợp lệ: Admin, ProjectManager, Member, Viewer
    }
}