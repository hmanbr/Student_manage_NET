﻿using System;
using System.Collections.Generic;

namespace G3.Models
{
    public partial class Issue
    {
        public int Id { get; set; }
        public int Iid { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string Status { get; set; } = null!;
        public DateTime? ClosedAt { get; set; }
        public int? MilestoneId { get; set; }
        public int? ClosedById { get; set; }
        public int AuthorId { get; set; }
        public int AssigneeId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int? ProjectId { get; set; }
        public string? Complexity { get; set; }
        public string? Quality { get; set; }

        public virtual GitLabUser Assignee { get; set; } = null!;
        public virtual GitLabUser Author { get; set; } = null!;
        public virtual GitLabUser? ClosedBy { get; set; }
        public virtual Milestone? Milestone { get; set; }
        public virtual Project? Project { get; set; }
    }
}
