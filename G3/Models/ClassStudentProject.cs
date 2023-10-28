using System;
using System.Collections.Generic;

namespace G3.Models
{
    public partial class ClassStudentProject
    {
        public int Id { get; set; }
        public bool? Status { get; set; }
        public int UserId { get; set; }
        public int ProjectId { get; set; }
        public int ClassId { get; set; }

        public virtual Class Class { get; set; } = null!;
        public virtual Project Project { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
