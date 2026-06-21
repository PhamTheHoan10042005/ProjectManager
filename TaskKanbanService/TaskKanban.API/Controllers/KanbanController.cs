using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskKanban.API.DTOs;
using TaskKanban.API.Services;

namespace TaskKanban.API.Controllers;

[ApiController]
[Route("api/projects/{projectId:guid}/kanban")]
[Authorize]
public class KanbanController(IKanbanService kanbanService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(KanbanBoardResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<KanbanBoardResponse>> GetBoard(
        Guid projectId,
        [FromQuery] Guid? sprintId,
        CancellationToken cancellationToken)
        => Ok(await kanbanService.GetBoardAsync(projectId, sprintId, cancellationToken));
}
