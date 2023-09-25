using System;
namespace G3.Dtos {
    public class ChangePasswordDto {
        [Required]
        [MinLength(8)]
        [MaxLength(64)]
        public string OldPassword { get; set; } = null!;

        [Required]
        [MinLength(8)]
        [MaxLength(64)]
        public string NewPassword { get; set; } = null!;

        [Required]
        [MinLength(8)]
        [MaxLength(64)]
        public string ConfirmPassword { get; set; } = null!;
    }
}

