using System;
using System.Collections.Generic;

namespace G3.Models
{
    public partial class Class
    {
        public Class()
        {
            ClassSettings = new HashSet<ClassSetting>();
            Milestones = new HashSet<Milestone>();
            Projects = new HashSet<Project>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int? SubjectId { get; set; }
        public string Status { get; set; } = null!;

        public virtual Subject? Subject { get; set; }
        public virtual ICollection<ClassSetting> ClassSettings { get; set; }
        public virtual ICollection<Milestone> Milestones { get; set; }
        public virtual ICollection<Project> Projects { get; set; }
    }
}
