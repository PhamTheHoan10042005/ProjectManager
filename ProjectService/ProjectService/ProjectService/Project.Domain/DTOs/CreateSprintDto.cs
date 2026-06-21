namespace Project.Domain.DTOs
{
    public class CreateSprintDto
    {
        public string Name { get; set; }
        public string? Goal { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
