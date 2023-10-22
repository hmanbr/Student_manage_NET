using System;
using System.Collections.Generic;

namespace G3.Models
{
    public partial class Project
    {
        public Project()
        {
            ClassStudentProjects = new HashSet<ClassStudentProject>();
            Issues = new HashSet<Issue>();
            Milestones = new HashSet<Milestone>();
        }

        public int Id { get; set; }
        public string ProjectCode { get; set; } = null!;
        public string EnglishName { get; set; } = null!;
        public string VietNameseName { get; set; } = null!;
        public string ProjectStatus { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string GroupName { get; set; } = null!;
        public int MentorId { get; set; }
        public int? GroupId { get; set; }

        public virtual Class? Group { get; set; }
        public virtual User Mentor { get; set; } = null!;
        public virtual ICollection<ClassStudentProject> ClassStudentProjects { get; set; }
        public virtual ICollection<Issue> Issues { get; set; }
        public virtual ICollection<Milestone> Milestones { get; set; }
    }
}
