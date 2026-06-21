using CommentNotifyService.Data;
using CommentNotifyService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace CommentNotifyService.Controllers
{
    [Route("api/users")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> ListUsers()
        {
            var users = await _context.Users
                .OrderBy(u => u.FullName)
                .Select(u => new
                {
                    id = u.Id,
                    email = u.Username,
                    fullName = u.FullName,
                    role = u.Role
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpPatch("{id}/role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateRole(Guid id, [FromBody] UpdateUserRoleDto dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound(new { message = "Không tìm thấy người dùng!" });

            var allowedRoles = new[] { "Admin", "ProjectManager", "Member", "Viewer" };
            if (!allowedRoles.Contains(dto.Role))
            {
                return BadRequest(new { message = "Role không hợp lệ!" });
            }

            user.Role = dto.Role;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                id = user.Id,
                email = user.Username,
                fullName = user.FullName,
                role = user.Role
            });
        }
    }

    public class UpdateUserRoleDto
    {
        [JsonPropertyName("role")]
        public string Role { get; set; } = "Viewer";
    }
}
