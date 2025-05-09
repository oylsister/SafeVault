using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SafeVault.Core
{
    public class SafeVaultContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public SafeVaultContext(DbContextOptions<SafeVaultContext> options) : base(options)
        {
        }

        /*
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite("Data Source=mydatabase.db");
        }
        */
    }

    public class User
    {
        public int UserID { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public string Role { get; set; } = "User"; // Default role
    }
}
