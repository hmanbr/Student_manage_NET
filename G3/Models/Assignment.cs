using System;
using System.Collections.Generic;

namespace G3.Models
{
    public partial class Assignment
    {
        public Assignment()
        {
            ClassAssignments = new HashSet<ClassAssignment>();
        }

        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public int SubjectId { get; set; }

        public virtual Subject Subject { get; set; } = null!;
        public virtual ICollection<ClassAssignment> ClassAssignments { get; set; }
    }
}
