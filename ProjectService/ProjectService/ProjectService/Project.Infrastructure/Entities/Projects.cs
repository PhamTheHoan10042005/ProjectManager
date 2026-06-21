using System;
using System.Collections.Generic;

namespace Project.Infrastructure.Entities
{
    public class Projects
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Color { get; set; }
        public DateTime? CreatedAt { get; set; }

        public ICollection<Sprint> Sprints { get; set; }
        public ICollection<ProjectMember> ProjectMembers { get; set; }
    }
}
