using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SafeVault;
using SafeVault.Core;
using System.Linq;

namespace SafeVault.Tests
{
    [TestFixture]
    public class SqlInjectionTests : IDisposable
    {
        private SafeVaultContext _context;

        [SetUp]
        public void Setup()
        {
            // Use an in-memory SQLite database for testing  
            var options = new DbContextOptionsBuilder<SafeVaultContext>()
                .UseSqlite("Data Source=:memory:") // In-memory SQLite database  
                .Options;

            _context = new SafeVaultContext(options);
            _context.Database.OpenConnection(); // Open the in-memory database connection  
            _context.Database.EnsureCreated(); // Create the schema in the in-memory database  

            // Seed the database with a test user  
            _context.Users.Add(new User { Username = "TestUser", Email = "testuser@example.com", Password = "popoporesx" });
            _context.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            Dispose();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }

        [Test]
        public void TestSqlInjectionInUsername()
        {
            // Attempt SQL injection via the Username field  
            string maliciousInput = "'; DROP TABLE Users; --";

            var user = new User
            {
                Username = maliciousInput,
                Email = "malicious@example.com",
                Password = "maliciousPassword"
            };

            // Add the malicious user  
            _context.Users.Add(user);
            Assert.DoesNotThrow(() => _context.SaveChanges(), "SQL injection attempt should not crash the application.");

            // Verify that the Users table still exists and contains the original user  
            var users = _context.Users.ToList();
            Assert.That(users.Any(u => u.Username == "TestUser"), Is.True, "Original user should still exist.");
            Assert.That(users.Any(u => u.Username == maliciousInput), Is.True, "Malicious input should be treated as a normal string.");
        }

        [Test]
        public void TestSqlInjectionInEmail()
        {
            // Attempt SQL injection via the Email field  
            string maliciousInput = "'; DROP TABLE Users; --";

            var user = new User
            {
                Username = "MaliciousUser",
                Email = maliciousInput,
                Password = "maliciousPassword"
            };

            // Add the malicious user  
            _context.Users.Add(user);
            Assert.DoesNotThrow(() => _context.SaveChanges(), "SQL injection attempt should not crash the application.");

            // Verify that the Users table still exists and contains the original user  
            var users = _context.Users.ToList();
            Assert.That(users.Any(u => u.Username == "TestUser"), Is.True, "Original user should still exist.");
            Assert.That(users.Any(u => u.Email == maliciousInput), Is.True, "Malicious input should be treated as a normal string.");
        }

        [Test]
        public void TestSqlInjectionInPassword()
        {
            // Attempt SQL injection via the Email field  
            string maliciousInput = "'; DROP TABLE Users; --";

            var user = new User
            {
                Username = "MaliciousUser",
                Email = "malicious@example.com",
                Password = maliciousInput
            };

            // Add the malicious user  
            _context.Users.Add(user);
            Assert.DoesNotThrow(() => _context.SaveChanges(), "SQL injection attempt should not crash the application.");

            // Verify that the Users table still exists and contains the original user  
            var users = _context.Users.ToList();
            Assert.That(users.Any(u => u.Username == "TestUser"), Is.True, "Original user should still exist.");
            Assert.That(users.Any(u => u.Password == maliciousInput), Is.True, "Malicious input should be treated as a normal string.");
        }
    }
}
