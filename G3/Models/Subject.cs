using System;
using System.Collections.Generic;

namespace G3.Models
{
    public partial class Subject
    {
        public Subject()
        {
            SubjectSettings = new HashSet<SubjectSetting>();
        }

        public int Id { get; set; }
        public string SubjectCode { get; set; } = null!;
        public string Name { get; set; } = null!;
        public bool? Status { get; set; }
        public int MentorId { get; set; }

        public virtual User Mentor { get; set; } = null!;
        public virtual ICollection<SubjectSetting> SubjectSettings { get; set; }
    }
}
