using System.ComponentModel.DataAnnotations;

namespace SafeVault.Core.Models
{
    public class UserInputModel
    {
        [Required]
        [StringLength(20, MinimumLength = 3)]
        [RegularExpression(@"^[A-Za-z0-9]+$", ErrorMessage = "Only letters and numbers are allowed.")]
        public required string Username { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 6)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[A-Za-z\d]+$", ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, and one number.")]
        public required string Password { get; set; }
        public string Role { get; set; } = "User"; // Default role is User
    }
}
