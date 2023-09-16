using System;
using System.Collections.Generic;

namespace G3.Models
{
    public partial class Setting
    {
        public Setting()
        {
            UserDomainSettings = new HashSet<User>();
            UserRoleSettings = new HashSet<User>();
        }

        public int SettingId { get; set; }
        public string Type { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Value { get; set; } = null!;
        public bool? IsActive { get; set; }

        public virtual ICollection<User> UserDomainSettings { get; set; }
        public virtual ICollection<User> UserRoleSettings { get; set; }
    }
}
