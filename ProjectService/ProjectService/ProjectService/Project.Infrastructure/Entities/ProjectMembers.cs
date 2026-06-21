namespace Project.Infrastructure.Entities
{
    public class ProjectMembers
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int UserId { get; set; }
        public string Role { get; set; }

        public Projects Project { get; set; }
    }
}
