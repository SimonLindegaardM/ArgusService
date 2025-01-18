using ArgusService.Models;
using System.ComponentModel.DataAnnotations;

namespace ArgusService_UnitTest
{
    [TestClass]
    public class UserTests
    {

        private const string LongString128 = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"; // 128 'a's
        private const string LongString129 = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"; // 129 'a's
        private const string LongString100 = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"; // 100 'a's
        private const string LongString101 = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"; // 101 'a's

        private User CreateUser(string firebaseUID, string email, string role, string notificationPreferences)
        {
            return new User
            {
                FirebaseUID = firebaseUID,
                Email = email,
                Role = role,
                NotificationPreferences = notificationPreferences
            };
        }

        private bool TryValidateModel(User user, out ICollection<ValidationResult> results)
        {
            var context = new ValidationContext(user, serviceProvider: null, items: null);
            results = new List<ValidationResult>();
            return Validator.TryValidateObject(user, context, results, true);
        }

        // FirebaseUID Tests
        [DataTestMethod]
        [DataRow("UID123")] // Valid
        [DataRow("")] // Lower valid limit
        [DataRow(LongString128)] // Upper valid limit
        public void Should_Accept_ValidFirebaseUID(string firebaseUID)
        {
            var user = CreateUser(firebaseUID, "test@example.com", "user", "EmailOnly");
            var result = TryValidateModel(user, out var validationResults);
            Assert.IsTrue(result);
            Assert.AreEqual(0, validationResults.Count);
        }

        [DataTestMethod]
        [DataRow("")] // Empty string
        [DataRow(null)] // Null
        [DataRow(LongString129)] // Exceeds max limit
        public void Should_ThrowException_ForInvalidFirebaseUID(string firebaseUID)
        {
            var user = CreateUser(firebaseUID, "test@example.com", "user", "EmailOnly");
            var result = TryValidateModel(user, out var validationResults);
            Assert.IsFalse(result);
            Assert.IsTrue(validationResults.Count > 0);
        }

        // Email Tests
        [DataTestMethod]
        [DataRow("test@example.com")] // Valid
        [DataRow("a@b.com")] // Lower valid limit
        public void Should_Accept_ValidEmail(string email)
        {
            var user = CreateUser("UID123", email, "user", "EmailOnly");
            var result = TryValidateModel(user, out var validationResults);
            Assert.IsTrue(result);
            Assert.AreEqual(0, validationResults.Count);
        }

        [DataTestMethod]
        [DataRow("")] // Empty string
        [DataRow(null)] // Null
        [DataRow("plainaddress")] // Invalid format
        [DataRow("@domain.com")] // Missing local part
        [DataRow("test@")] // Missing domain
        public void Should_ThrowException_ForInvalidEmail(string email)
        {
            var user = CreateUser("UID123", email, "user", "EmailOnly");
            var result = TryValidateModel(user, out var validationResults);
            Assert.IsFalse(result);
            Assert.IsTrue(validationResults.Count > 0);
        }

        // Role Tests
        [DataTestMethod]
        [DataRow("user")] // Valid
        [DataRow("admin")] // Valid
        public void Should_Accept_ValidRole(string role)
        {
            var user = CreateUser("UID123", "test@example.com", role, "EmailOnly");
            var result = TryValidateModel(user, out var validationResults);
            Assert.IsTrue(result);
            Assert.AreEqual(0, validationResults.Count);
        }

        [DataTestMethod]
        [DataRow("")] // Empty string
        [DataRow(null)] // Null
        [DataRow("usr")] // Too short
        [DataRow("admins")] // Too long
        [DataRow("manager")] // Invalid value
        public void Should_ThrowException_ForInvalidRole(string role)
        {
            var user = CreateUser("UID123", "test@example.com", role, "EmailOnly");
            var result = TryValidateModel(user, out var validationResults);
            Assert.IsFalse(result);
            Assert.IsTrue(validationResults.Count > 0);
        }

        // NotificationPreferences Tests
        [DataTestMethod]
        [DataRow("EmailOnly")] // Valid
        [DataRow("SMSOnly")] // Valid
        [DataRow("")] // Valid (optional)
        [DataRow(null)] // Valid (optional)
        public void Should_Accept_ValidNotificationPreferences(string notificationPreferences)
        {
            var user = CreateUser("UID123", "test@example.com", "user", notificationPreferences);
            var result = TryValidateModel(user, out var validationResults);
            Assert.IsTrue(result);
            Assert.AreEqual(0, validationResults.Count);
        }
    }
}