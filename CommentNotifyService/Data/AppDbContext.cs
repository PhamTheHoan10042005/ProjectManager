using CommentNotifyService.Models;
using Microsoft.EntityFrameworkCore;

namespace CommentNotifyService.Data
{
    public class AppDbContext : DbContext
    {
        // Hàm khởi tạo (Constructor) để truyền cấu hình từ file appsettings.json vào
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Khai báo các bảng dữ liệu sẽ được tạo trong SQL Server
        public DbSet<User> Users { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }
    }
}