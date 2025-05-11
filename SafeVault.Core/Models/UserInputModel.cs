using System.ComponentModel.DataAnnotations;

namespace SafeVault.Core.Models
{
    public class UserInputModel
    {
        [Required]
        [StringLength(20, MinimumLength = 3)]
        public required string Username { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 6)]
        public required string Password { get; set; }
        public string Role { get; set; } = "User"; // Default role is User
    }
}
