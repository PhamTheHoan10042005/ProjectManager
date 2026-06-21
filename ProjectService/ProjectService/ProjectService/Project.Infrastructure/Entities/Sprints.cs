using System;

namespace Project.Infrastructure.Entities
{
    public class Sprints
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string Name { get; set; }
        public string Goal { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }

        public Projects Project { get; set; }
    }
}
