using System;
using System.Collections.Generic;

namespace G3.Models
{
    public partial class User
    {
        public User()
        {
            Subjects = new HashSet<Subject>();
        }

        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public int DomainSettingId { get; set; }
        public int RoleSettingId { get; set; }
        public string? Hash { get; set; }
        public bool? Status { get; set; }
        public string? ConfirmToken { get; set; }
        public DateTime? ConfirmTokenVerifyAt { get; set; }
        public string? ResetPassToken { get; set; }
        public string? Avatar { get; set; }
        public string Name { get; set; } = null!;
        public DateTime? DateOfBirth { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public bool? Gender { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual Setting DomainSetting { get; set; } = null!;
        public virtual Setting RoleSetting { get; set; } = null!;
        public virtual Gitlabuser? Gitlabuser { get; set; }
        public virtual ICollection<Subject> Subjects { get; set; }
    }
}
