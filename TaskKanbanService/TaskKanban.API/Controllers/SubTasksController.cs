using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskKanban.API.DTOs;
using TaskKanban.API.Services;

namespace TaskKanban.API.Controllers;

[ApiController]
[Route("api/projects/{projectId:guid}/tasks/{taskId:guid}/subtasks")]
[Authorize]
public class SubTasksController(ISubTaskService subTaskService, ITaskService taskService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<SubTaskResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<SubTaskResponse>>> GetSubTasks(
        Guid projectId,
        Guid taskId,
        CancellationToken cancellationToken)
    {
        if (!await TaskBelongsToProject(projectId, taskId, cancellationToken))
            return NotFound();

        return Ok(await subTaskService.GetByTaskAsync(taskId, cancellationToken));
    }

    [HttpPost]
    [ProducesResponseType(typeof(SubTaskResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SubTaskResponse>> CreateSubTask(
        Guid projectId,
        Guid taskId,
        [FromBody] CreateSubTaskRequest request,
        CancellationToken cancellationToken)
    {
        if (!await TaskBelongsToProject(projectId, taskId, cancellationToken))
            return NotFound();

        var subTask = await subTaskService.CreateAsync(taskId, request, cancellationToken);
        if (subTask is null) return NotFound();

        return CreatedAtAction(nameof(GetSubTasks), new { projectId, taskId }, subTask);
    }

    [HttpPut("{subTaskId:guid}")]
    [ProducesResponseType(typeof(SubTaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SubTaskResponse>> UpdateSubTask(
        Guid projectId,
        Guid taskId,
        Guid subTaskId,
        [FromBody] UpdateSubTaskRequest request,
        CancellationToken cancellationToken)
    {
        if (!await TaskBelongsToProject(projectId, taskId, cancellationToken))
            return NotFound();

        var subTask = await subTaskService.UpdateAsync(taskId, subTaskId, request, cancellationToken);
        return subTask is null ? NotFound() : Ok(subTask);
    }

    [HttpDelete("{subTaskId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSubTask(
        Guid projectId,
        Guid taskId,
        Guid subTaskId,
        CancellationToken cancellationToken)
    {
        if (!await TaskBelongsToProject(projectId, taskId, cancellationToken))
            return NotFound();

        var deleted = await subTaskService.DeleteAsync(taskId, subTaskId, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    private async Task<bool> TaskBelongsToProject(Guid projectId, Guid taskId, CancellationToken cancellationToken)
    {
        var task = await taskService.GetByIdAsync(taskId, cancellationToken);
        return task is not null && task.ProjectId == projectId;
    }
}
