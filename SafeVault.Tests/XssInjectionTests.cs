using NUnit.Framework;

namespace SafeVault.Tests
{
    [TestFixture]
    public class XssInjectionTests
    {
        private readonly UserInputValidator _validator = new UserInputValidator();

        [Test]
        public void TestBasicXssInjectionAttempt()
        {
            string maliciousInput = "<script>alert('Hacked!');</script>";
            bool result = _validator.IsValid(maliciousInput);

            Assert.That(result, Is.False, "XSS attack should be blocked.");
        }

        [Test]
        public void TestEventBasedXssInjection()
        {
            string maliciousInput = "<img src='x' onerror='alert(\"XSS\")'>";
            bool result = _validator.IsValid(maliciousInput);

            Assert.That(result, Is.False, "Event-based XSS should be rejected.");
        }
    }

    public class UserInputValidator
    {
        public bool IsValid(string input)
        {
            return !(input.Contains("<script>") || input.Contains("</script>") || input.Contains("onerror"));
        }
    }
}