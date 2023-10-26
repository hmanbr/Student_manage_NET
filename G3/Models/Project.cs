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
            Summits = new HashSet<Summit>();
        }

        public int Id { get; set; }
        public string ProjectCode { get; set; } = null!;
        public string EnglishName { get; set; } = null!;
        public string GroupName { get; set; } = null!;
        public string? VietNameseName { get; set; }
        public string Status { get; set; } = null!;
        public string? Description { get; set; }
        public int? MentorId { get; set; }
        public int? ClassId { get; set; }

        public virtual Class? Class { get; set; }
        public virtual User? Mentor { get; set; }
        public virtual ICollection<ClassStudentProject> ClassStudentProjects { get; set; }
        public virtual ICollection<Issue> Issues { get; set; }
        public virtual ICollection<Milestone> Milestones { get; set; }
        public virtual ICollection<Summit> Summits { get; set; }
    }
}
