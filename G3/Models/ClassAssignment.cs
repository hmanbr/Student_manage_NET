using System;
using System.Collections.Generic;

namespace G3.Models
{
    public partial class ClassAssignment
    {
        public ClassAssignment()
        {
            Summits = new HashSet<Summit>();
        }

        public string Key { get; set; } = null!;
        public int AssignmentId { get; set; }
        public int ClassId { get; set; }

        public virtual Assignment Assignment { get; set; } = null!;
        public virtual Class Class { get; set; } = null!;
        public virtual ICollection<Summit> Summits { get; set; }
    }
}
