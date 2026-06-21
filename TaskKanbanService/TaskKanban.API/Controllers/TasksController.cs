using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskKanban.API.DTOs;
using TaskKanban.API.Services;

namespace TaskKanban.API.Controllers;

[ApiController]
[Route("api/projects/{projectId:guid}/tasks")]
[Authorize]
public class TasksController(ITaskService taskService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<TaskResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<TaskResponse>>> GetTasks(
        Guid projectId,
        [FromQuery] Guid? sprintId,
        CancellationToken cancellationToken)
        => Ok(await taskService.GetByProjectAsync(projectId, sprintId, cancellationToken));

    [HttpGet("{taskId:guid}")]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskResponse>> GetTask(
        Guid projectId,
        Guid taskId,
        CancellationToken cancellationToken)
    {
        var task = await taskService.GetByIdAsync(taskId, cancellationToken);
        if (task is null || task.ProjectId != projectId)
            return NotFound();

        return Ok(task);
    }

    [HttpPost]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<TaskResponse>> CreateTask(
        Guid projectId,
        [FromBody] CreateTaskRequest request,
        CancellationToken cancellationToken)
    {
        var task = await taskService.CreateAsync(projectId, request, cancellationToken);
        return CreatedAtAction(nameof(GetTask), new { projectId, taskId = task.Id }, task);
    }

    [HttpPut("{taskId:guid}")]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskResponse>> UpdateTask(
        Guid projectId,
        Guid taskId,
        [FromBody] UpdateTaskRequest request,
        CancellationToken cancellationToken)
    {
        var existing = await taskService.GetByIdAsync(taskId, cancellationToken);
        if (existing is null || existing.ProjectId != projectId)
            return NotFound();

        var task = await taskService.UpdateAsync(taskId, request, cancellationToken);
        return Ok(task);
    }

    [HttpPatch("{taskId:guid}/assign")]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskResponse>> AssignTask(
        Guid projectId,
        Guid taskId,
        [FromBody] AssignTaskRequest request,
        CancellationToken cancellationToken)
    {
        var existing = await taskService.GetByIdAsync(taskId, cancellationToken);
        if (existing is null || existing.ProjectId != projectId)
            return NotFound();

        var userId = GetCurrentUserId();
        var task = await taskService.AssignAsync(taskId, request, userId, cancellationToken);
        return Ok(task);
    }

    [HttpPatch("{taskId:guid}/status")]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskResponse>> UpdateStatus(
        Guid projectId,
        Guid taskId,
        [FromBody] UpdateTaskStatusRequest request,
        CancellationToken cancellationToken)
    {
        var existing = await taskService.GetByIdAsync(taskId, cancellationToken);
        if (existing is null || existing.ProjectId != projectId)
            return NotFound();

        var userId = GetCurrentUserId();
        var task = await taskService.UpdateStatusAsync(taskId, request, userId, cancellationToken);
        return Ok(task);
    }

    [HttpPatch("{taskId:guid}/move")]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskResponse>> MoveTask(
        Guid projectId,
        Guid taskId,
        [FromBody] MoveTaskRequest request,
        CancellationToken cancellationToken)
    {
        var existing = await taskService.GetByIdAsync(taskId, cancellationToken);
        if (existing is null || existing.ProjectId != projectId)
            return NotFound();

        var userId = GetCurrentUserId();
        var task = await taskService.MoveAsync(taskId, request, userId, cancellationToken);
        return Ok(task);
    }

    [HttpDelete("{taskId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTask(
        Guid projectId,
        Guid taskId,
        CancellationToken cancellationToken)
    {
        var existing = await taskService.GetByIdAsync(taskId, cancellationToken);
        if (existing is null || existing.ProjectId != projectId)
            return NotFound();

        await taskService.DeleteAsync(taskId, cancellationToken);
        return NoContent();
    }

    private Guid? GetCurrentUserId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        return Guid.TryParse(claim, out var userId) ? userId : null;
    }
}
