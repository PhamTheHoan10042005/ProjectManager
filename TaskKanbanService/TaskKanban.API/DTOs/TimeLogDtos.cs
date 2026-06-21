using System.ComponentModel.DataAnnotations;

namespace TaskKanban.API.DTOs;

public record TimeLogResponse(
    Guid Id,
    Guid TaskId,
    Guid UserId,
    decimal Hours,
    string? Description,
    DateTime LoggedDate,
    DateTime CreatedAt);

public class CreateTimeLogRequest
{
    [Required]
    public Guid UserId { get; set; }

    [Range(0.25, 24)]
    public decimal Hours { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    [Required]
    public DateTime LoggedDate { get; set; }
}

public record TaskTimeSummaryResponse(Guid TaskId, decimal TotalHours, IReadOnlyList<TimeLogResponse> Logs);
