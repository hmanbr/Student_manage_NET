using System;
using System.Collections.Generic;

namespace G3.Models
{
    public partial class Milestone
    {
        public Milestone()
        {
            Issues = new HashSet<Issue>();
        }

        public int Id { get; set; }
        public int Iid { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime DueDate { get; set; }
        public int? GroupId { get; set; }
        public int? ProjectId { get; set; }
        public DateTime StartDate { get; set; }
        public string State { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual Class? Group { get; set; }
        public virtual Project? Project { get; set; }
        public virtual ICollection<Issue> Issues { get; set; }
    }
}
