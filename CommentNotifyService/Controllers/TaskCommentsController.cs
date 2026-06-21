using CommentNotifyService.Data;
using CommentNotifyService.DTOs;
using CommentNotifyService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CommentNotifyService.Controllers
{
    [Route("api/projects/{projectId}/tasks/{taskId}/comments")]
    [ApiController]
    public class TaskCommentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TaskCommentsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetComments(Guid projectId, Guid taskId)
        {
            var comments = await (
                from c in _context.Comments
                join u in _context.Users on c.UserId equals u.Id into userJoin
                from u in userJoin.DefaultIfEmpty()
                where c.TaskId == taskId
                orderby c.CreatedAt descending
                select new
                {
                    id = c.Id,
                    taskId = c.TaskId,
                    userId = c.UserId,
                    userName = u != null ? u.FullName : (string?)null,
                    content = c.Content,
                    createdAt = c.CreatedAt
                }).ToListAsync();

            return Ok(comments);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddComment(Guid projectId, Guid taskId, [FromBody] AddCommentDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "Không tìm thấy thông tin người dùng trong Token!" });
            }

            var user = await _context.Users.FindAsync(userId);
            var comment = new Comment
            {
                TaskId = taskId,
                UserId = userId,
                Content = dto.Content,
                CreatedAt = DateTime.UtcNow
            };

            _context.Comments.Add(comment);
            _context.ActivityLogs.Add(new ActivityLog
            {
                ProjectId = projectId,
                UserId = userId,
                UserName = user?.FullName,
                Action = "comment_added",
                Description = $"Đã thêm bình luận vào task {taskId}"
            });
            await _context.SaveChangesAsync();

            return Ok(new
            {
                id = comment.Id,
                taskId = comment.TaskId,
                userId = comment.UserId,
                userName = user?.FullName,
                content = comment.Content,
                createdAt = comment.CreatedAt
            });
        }
    }
}
