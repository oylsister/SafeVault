using System.ComponentModel.DataAnnotations;

namespace SafeVault.Core.Models
{
    public class LoginInputModel
    {
        [Required]
        public required string Username { get; set; }

        [Required]
        public required string Password { get; set; }
    }
}
