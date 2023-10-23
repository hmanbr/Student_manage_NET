using System;
using System.Collections.Generic;

namespace G3.Models
{
    public partial class Gitlabuser
    {
        public Gitlabuser()
        {
            IssueAssignees = new HashSet<Issue>();
            IssueAuthors = new HashSet<Issue>();
            IssueClosedBies = new HashSet<Issue>();
        }

        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string State { get; set; } = null!;
        public bool Locked { get; set; }
        public string? AvatarUrl { get; set; }
        public string WebUrl { get; set; } = null!;
        public int? UserId { get; set; }

        public virtual User? User { get; set; }
        public virtual ICollection<Issue> IssueAssignees { get; set; }
        public virtual ICollection<Issue> IssueAuthors { get; set; }
        public virtual ICollection<Issue> IssueClosedBies { get; set; }
    }
}
