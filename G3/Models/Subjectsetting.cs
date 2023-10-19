using System;
using System.Collections.Generic;

namespace G3.Models
{
    public partial class SubjectSetting
    {
        public int Id { get; set; }
        public string Type { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Value { get; set; } = null!;
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
        public int? SubjectId { get; set; }

        public virtual Subject? Subject { get; set; }
    }
}
