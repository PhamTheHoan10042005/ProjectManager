using System.ComponentModel.DataAnnotations;

namespace TaskKanban.API.DTOs;

public record SubTaskResponse(
    Guid Id,
    Guid TaskId,
    string Title,
    bool IsCompleted,
    int OrderIndex,
    DateTime CreatedAt);

public class CreateSubTaskRequest
{
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;
}

public class UpdateSubTaskRequest
{
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    public bool IsCompleted { get; set; }
}
