using System;
namespace G3.Dtos {
    public class SignInDto {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [MinLength(8)]
        [MaxLength(64)]
        public string Password { get; set; } = null!;
    }
}

