using CommentNotifyService.Data;
using CommentNotifyService.DTOs;
using CommentNotifyService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CommentNotifyService.Controllers
{
    [Route("api/notifications")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public NotificationsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("consume-task-event")]
        public async Task<IActionResult> ConsumeTaskEvent([FromBody] TaskEventDto eventDto)
        {
            var notification = new Notification
            {
                UserId = eventDto.AssigneeId,
                Title = $"Cập nhật công việc: {eventDto.TaskTitle}",
                Message = $"Người dùng [{eventDto.ActionBy}] đã cập nhật trạng thái công việc sang: {eventDto.Status}.",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Hệ thống Comment & Notify đã consume event và tạo thông báo thành công!",
                createdNotification = notification
            });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetMyNotifications([FromQuery] bool unreadOnly = false)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "Không tìm thấy thông tin người dùng trong Token!" });
            }

            var query = _context.Notifications
                .Where(n => n.UserId == userId);

            if (unreadOnly)
            {
                query = query.Where(n => !n.IsRead);
            }

            var notifications = await query
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new
                {
                    id = n.Id,
                    userId = n.UserId,
                    title = n.Title,
                    message = n.Message,
                    isRead = n.IsRead,
                    createdAt = n.CreatedAt
                })
                .ToListAsync();

            return Ok(notifications);
        }

        [HttpPatch("{id}/read")]
        [Authorize]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
            {
                return NotFound(new { message = "Không tìm thấy thông báo này!" });
            }

            notification.IsRead = true;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                id = notification.Id,
                userId = notification.UserId,
                title = notification.Title,
                message = notification.Message,
                isRead = notification.IsRead,
                createdAt = notification.CreatedAt
            });
        }

        [HttpPost("read-all")]
        [Authorize]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "Không tìm thấy thông tin người dùng trong Token!" });
            }

            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Đã đánh dấu tất cả thông báo là đã đọc." });
        }
    }
}
