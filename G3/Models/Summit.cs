﻿using System;
using System.Collections.Generic;

namespace G3.Models
{
    public partial class Summit
    {
        public int Id { get; set; }
        public string FileUrl { get; set; } = null!;
        public DateTime SummitTime { get; set; }
        public decimal? Grade { get; set; }
        public string? Comment { get; set; }
        public DateTime? CommentTime { get; set; }
        public int ProjectId { get; set; }
        public string ClassAssignmentId { get; set; } = null!;

        public virtual ClassAssignment ClassAssignment { get; set; } = null!;
        public virtual Project Project { get; set; } = null!;
    }
}
