using System;
namespace G3.Dtos {
    public class ForgotPasswordDto {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
    }
}

