using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskKanban.API.DTOs;
using TaskKanban.API.Services;

namespace TaskKanban.API.Controllers;

[ApiController]
[Route("api/projects/{projectId:guid}/tasks/{taskId:guid}/timelogs")]
[Authorize]
public class TimeLogsController(ITimeLogService timeLogService, ITaskService taskService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(TaskTimeSummaryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskTimeSummaryResponse>> GetTimeLogs(
        Guid projectId,
        Guid taskId,
        CancellationToken cancellationToken)
    {
        if (!await TaskBelongsToProject(projectId, taskId, cancellationToken))
            return NotFound();

        var summary = await timeLogService.GetByTaskAsync(taskId, cancellationToken);
        return summary is null ? NotFound() : Ok(summary);
    }

    [HttpPost]
    [ProducesResponseType(typeof(TimeLogResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TimeLogResponse>> CreateTimeLog(
        Guid projectId,
        Guid taskId,
        [FromBody] CreateTimeLogRequest request,
        CancellationToken cancellationToken)
    {
        if (!await TaskBelongsToProject(projectId, taskId, cancellationToken))
            return NotFound();

        var timeLog = await timeLogService.CreateAsync(taskId, request, cancellationToken);
        if (timeLog is null) return NotFound();

        return CreatedAtAction(nameof(GetTimeLogs), new { projectId, taskId }, timeLog);
    }

    [HttpDelete("{timeLogId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTimeLog(
        Guid projectId,
        Guid taskId,
        Guid timeLogId,
        CancellationToken cancellationToken)
    {
        if (!await TaskBelongsToProject(projectId, taskId, cancellationToken))
            return NotFound();

        var deleted = await timeLogService.DeleteAsync(taskId, timeLogId, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    private async Task<bool> TaskBelongsToProject(Guid projectId, Guid taskId, CancellationToken cancellationToken)
    {
        var task = await taskService.GetByIdAsync(taskId, cancellationToken);
        return task is not null && task.ProjectId == projectId;
    }
}
