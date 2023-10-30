using System;
using System.Collections.Generic;

namespace G3.Models
{
    public partial class Class
    {
        public Class()
        {
            ClassAssignments = new HashSet<ClassAssignment>();
            ClassSettings = new HashSet<ClassSetting>();
            ClassStudentProjects = new HashSet<ClassStudentProject>();
            Milestones = new HashSet<Milestone>();
            Projects = new HashSet<Project>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int? GitLabGroupId { get; set; }
        public int? SubjectId { get; set; }
        public bool? Status { get; set; }

        public virtual Subject? Subject { get; set; }
        public virtual ICollection<ClassAssignment> ClassAssignments { get; set; }
        public virtual ICollection<ClassSetting> ClassSettings { get; set; }
        public virtual ICollection<ClassStudentProject> ClassStudentProjects { get; set; }
        public virtual ICollection<Milestone> Milestones { get; set; }
        public virtual ICollection<Project> Projects { get; set; }
    }
}
