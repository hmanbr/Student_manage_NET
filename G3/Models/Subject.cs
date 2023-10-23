using System;
using System.Collections.Generic;

namespace G3.Models
{
    public partial class Subject
    {
        public Subject()
        {
            Assignments = new HashSet<Assignment>();
            Classes = new HashSet<Class>();
            Subjectsettings = new HashSet<Subjectsetting>();
        }

        public int Id { get; set; }
        public string SubjectCode { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool? Status { get; set; }
        public int? MentorId { get; set; }

        public virtual User? Mentor { get; set; }
        public virtual ICollection<Assignment> Assignments { get; set; }
        public virtual ICollection<Class> Classes { get; set; }
        public virtual ICollection<Subjectsetting> Subjectsettings { get; set; }
    }
}
