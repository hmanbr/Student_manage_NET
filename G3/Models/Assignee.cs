using System;
using System.Collections.Generic;

namespace G3.Models
{
    public partial class Assignee
    {
        public int IssueId { get; set; }
        public int GitLabUserId { get; set; }

        public virtual Gitlabuser GitLabUser { get; set; } = null!;
        public virtual Issue Issue { get; set; } = null!;
    }
}
