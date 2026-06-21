using CommentNotifyService.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CommentNotifyService.Controllers
{
    [Route("api/projects/{projectId}/activities")]
    [ApiController]
    public class ActivitiesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ActivitiesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetActivities(Guid projectId)
        {
            var activities = await _context.ActivityLogs
                .Where(a => a.ProjectId == projectId)
                .OrderByDescending(a => a.CreatedAt)
                .Select(a => new
                {
                    id = a.Id,
                    projectId = a.ProjectId,
                    userId = a.UserId,
                    userName = a.UserName,
                    action = a.Action,
                    description = a.Description,
                    createdAt = a.CreatedAt
                })
                .ToListAsync();

            return Ok(activities);
        }
    }
}
