﻿using System;
namespace G3.Dtos {
    public class ResetPasswordDto {
        [Required]
        [MinLength(8)]
        [MaxLength(64)]
        public string Password { get; set; } = null!;

        [Required]
        [MinLength(8)]
        [MaxLength(64)]
        public string ConfirmPassword { get; set; } = null!;
    }
}

