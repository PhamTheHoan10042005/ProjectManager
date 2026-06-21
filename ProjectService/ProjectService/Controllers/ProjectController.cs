using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Domain.DTOs;
using Project.Infrastructure.Data;
using Project.Infrastructure.Entities;
using ProjectEntity = Project.Infrastructure.Entities.Project;

namespace ProjectService.Controllers
{
    [ApiController]
    [Route("api/projects")]
    public class ProjectController : ControllerBase
    {
        private readonly ProjectDbContext _db;

        public ProjectController(ProjectDbContext db)
        {
            _db = db;
        }

        // GET api/projects
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var projects = await _db.Projects
                .Include(p => p.ProjectMembers)
                .ToListAsync();
            return Ok(projects);
        }

        // GET api/projects/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var project = await _db.Projects
                .Include(p => p.ProjectMembers)
                .Include(p => p.Sprints)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (project == null) return NotFound();
            return Ok(project);
        }

        // POST api/projects
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProjectDto dto)
        {
            var project = new ProjectEntity
            {
                Name = dto.Name,
                Description = dto.Description,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Color = dto.Color,
                CreatedAt = DateTime.UtcNow
            };
            _db.Projects.Add(project);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = project.Id }, project);
        }

        // PUT api/projects/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateProjectDto dto)
        {
            var project = await _db.Projects.FindAsync(id);
            if (project == null) return NotFound();

            project.Name = dto.Name;
            project.Description = dto.Description;
            project.StartDate = dto.StartDate;
            project.EndDate = dto.EndDate;
            project.Color = dto.Color;

            await _db.SaveChangesAsync();
            return Ok(project);
        }

        // DELETE api/projects/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var project = await _db.Projects.FindAsync(id);
            if (project == null) return NotFound();

            _db.Projects.Remove(project);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // ── MEMBERS ──────────────────────────────────────────────

        // GET api/projects/{projectId}/members
        [HttpGet("{projectId}/members")]
        public async Task<IActionResult> GetMembers(int projectId)
        {
            var members = await _db.ProjectMembers
                .Where(m => m.ProjectId == projectId)
                .ToListAsync();
            return Ok(members);
        }

        // POST api/projects/{projectId}/members
        [HttpPost("{projectId}/members")]
        public async Task<IActionResult> AddMember(int projectId, [FromBody] AddMemberDto dto)
        {
            var projectExists = await _db.Projects.AnyAsync(p => p.Id == projectId);
            if (!projectExists) return NotFound("Project not found");

            // BỔ SUNG ĐOẠN KIỂM TRA NÀY:
            var memberExists = await _db.ProjectMembers.AnyAsync(m => m.ProjectId == projectId && m.UserId == dto.UserId);
            if (memberExists)
            {
                return BadRequest(new { message = "Người dùng này đã là thành viên của dự án rồi!" });
            }

            var member = new ProjectMember
            {
                ProjectId = projectId,
                UserId = dto.UserId,
                Role = dto.Role
            };
            _db.ProjectMembers.Add(member);
            await _db.SaveChangesAsync();
            return Ok(member);
        }

        // PUT api/projects/{projectId}/members/{memberId}
        [HttpPut("{projectId}/members/{memberId}")]
        public async Task<IActionResult> UpdateMember(int projectId, int memberId, [FromBody] AddMemberDto dto)
        {
            var member = await _db.ProjectMembers
                .FirstOrDefaultAsync(m => m.Id == memberId && m.ProjectId == projectId);
            if (member == null) return NotFound();

            member.Role = dto.Role;
            await _db.SaveChangesAsync();
            return Ok(member);
        }

        // DELETE api/projects/{projectId}/members/{memberId}
        [HttpDelete("{projectId}/members/{memberId}")]
        public async Task<IActionResult> RemoveMember(int projectId, int memberId)
        {
            var member = await _db.ProjectMembers
                .FirstOrDefaultAsync(m => m.Id == memberId && m.ProjectId == projectId);
            if (member == null) return NotFound();

            _db.ProjectMembers.Remove(member);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // ── SPRINTS ──────────────────────────────────────────────

        // GET api/projects/{projectId}/sprints
        [HttpGet("{projectId}/sprints")]
        public async Task<IActionResult> GetSprints(int projectId)
        {
            var sprints = await _db.Sprints
                .Where(s => s.ProjectId == projectId)
                .ToListAsync();
            return Ok(sprints);
        }

        // POST api/projects/{projectId}/sprints
        [HttpPost("{projectId}/sprints")]
        public async Task<IActionResult> CreateSprint(int projectId, [FromBody] CreateSprintDto dto)
        {
            var projectExists = await _db.Projects.AnyAsync(p => p.Id == projectId);
            if (!projectExists) return NotFound("Project not found");

            var sprint = new Sprint
            {
                ProjectId = projectId,
                Name = dto.Name,
                Goal = dto.Goal,
                StartDate = dto.StartDate ?? DateTime.UtcNow,
                EndDate = dto.EndDate ?? DateTime.UtcNow.AddDays(14),
                Status = "PLANNING"
            };
            _db.Sprints.Add(sprint);
            await _db.SaveChangesAsync();
            return Ok(sprint);
        }

        // POST api/projects/{projectId}/sprints/{sprintId}/start  ← đổi từ PUT sang POST cho khớp FE
        [HttpPost("{projectId}/sprints/{sprintId}/start")]
        public async Task<IActionResult> StartSprint(int projectId, int sprintId)
        {
            var sprint = await _db.Sprints
                .FirstOrDefaultAsync(s => s.Id == sprintId && s.ProjectId == projectId);
            if (sprint == null) return NotFound();

            sprint.Status = "ACTIVE";
            await _db.SaveChangesAsync();
            return Ok(sprint);
        }
    }
}
