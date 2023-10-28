using System;
using System.Collections.Generic;

namespace G3.Models
{
    public partial class ClassAssignment
    {
        public ClassAssignment()
        {
            Submits = new HashSet<Submit>();
        }

        public string Key { get; set; } = null!;
        public int AssignmentId { get; set; }
        public int ClassId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public virtual Assignment Assignment { get; set; } = null!;
        public virtual Class Class { get; set; } = null!;
        public virtual ICollection<Submit> Submits { get; set; }
    }
}
