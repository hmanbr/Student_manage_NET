using System;
using System.Collections.Generic;

namespace G3.Models
{
    public partial class SubjectSetting
    {
        public int SubjectSettingId { get; set; }
        public string SubjectId { get; set; } = null!;
        public string Type { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Value { get; set; } = null!;
        public bool? IsActive { get; set; }

        public virtual Subject Subject { get; set; } = null!;
    }
}
