using System;
using System.Collections.Generic;

namespace G3.Dtos
{
    public partial class UpdateUserDto
    {
        public UpdateUserDto()
        {

        }
        public UpdateUserDto(User u)
        {
            Id = u.Id;
            RoleSettingId = u.RoleSettingId;
            Status = u.Status;
            Avatar = u.Avatar;
            Name = u.Name;
            DateOfBirth = u.DateOfBirth;
            Phone = u.Phone;
            Address = u.Address;
            Gender = u.Gender;
            Description = u.Description;
            UpdatedAt = u.UpdatedAt;
        }
        public int Id { get; set; }
        public int RoleSettingId { get; set; }
        public bool? Status { get; set; }
        public string? Avatar { get; set; }
        public string Name { get; set; } = null!;
        public DateTime? DateOfBirth { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public bool? Gender { get; set; }
        public string? Description { get; set; }
        public DateTime UpdatedAt { get; set; }
        public User toUser()
        {
            User u = new User();
            u.Id = Id;
            u.Status = Status;
            u.Avatar = Avatar;
            u.Name = Name;
            u.Phone = Phone;
            u.DateOfBirth = DateOfBirth;
            u.Gender = Gender;
            u.Address = Address;
            u.Description = Description;
            u.UpdatedAt = UpdatedAt;
            return u;
        }
        public void saveData(User user)
        {
            user.Status = Status;
            user.UpdatedAt = UpdatedAt;

            if (Avatar != null) user.Avatar = Avatar;
            if (Name != null) user.Name = Name;
            if (Phone != null) user.Phone = Phone;
            if (DateOfBirth != null) user.DateOfBirth = DateOfBirth;
            if (Gender != null) user.Gender = Gender;
            if (Address != null) user.Address = Address;
            if (Description != null) user.Description = Description;
        }
    }
}
