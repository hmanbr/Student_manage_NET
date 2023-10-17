using System;
using System.Collections.Generic;

namespace G3.Models
{
    public partial class Project
    {
        public Project()
        {
            Milestones = new HashSet<Milestone>();
        }

        public int Id { get; set; }
        public int? ClassId { get; set; }

        public virtual Class? Class { get; set; }
        public virtual ICollection<Milestone> Milestones { get; set; }
    }
}
