using ArgusService.Models;
using System.ComponentModel.DataAnnotations;

namespace ArgusService_UnitTest
{
    [TestClass]
    public class UserTests
    {

        private static readonly string LongValidFirebaseUID = new string('a', 128);
        private static readonly string LongValidFirebaseUIDminus1 = new string('a', 127);
        private static readonly string LongInvalidFirebaseUID = new string('a', 129);
        private static readonly string ValidMaxLengthEmail = new string('a', 93) + "@test.com";
        private static readonly string InvalidMaxLengthEmail = new string('a', 94) + "@test.com";

        // Helper Method for Validation
        private void ValidateModel(object model)
        {
            var context = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();
            if (!Validator.TryValidateObject(model, context, results, true))
            {
                throw new ValidationException(results[0].ErrorMessage);
            }
        }

        // FirebaseUID Tests

        //Valid test cases
        [DataTestMethod]
        [DataRow("UID123")] // Valid
        [DataRow("a")] // Lower valid limit
        [DataRow("aa")] // Lower valid limit plus 1
        [DataRow(nameof(LongValidFirebaseUID))] // Upper valid limit
        [DataRow(nameof(LongValidFirebaseUIDminus1))] // Upper valid limit minus 1
        public void Should_Accept_ValidFirebaseUID(string firebaseUID)
        {
            // Arrange & Act
            var user = new User { FirebaseUID = firebaseUID };

            // Assert
            Assert.AreEqual(firebaseUID, user.FirebaseUID);
        }

        //Invalid test cases
        [DataTestMethod]
        [DataRow("")] // Empty string
        [DataRow(null)] // Null
        [DataRow(nameof(LongInvalidFirebaseUID))] // Exceeds max limit
        public void Should_ThrowException_ForInvalidFirebaseUID(string firebaseUID)
        {
            // Arrange
            var user = new User { FirebaseUID = firebaseUID };

            // Act & Assert
            Assert.ThrowsException<ValidationException>(() => ValidateModel(user));
        }

        // Email Tests

        [DataTestMethod]
        [DataRow("test@example.com")] // Valid
        [DataRow("a@b.com")] // Lower valid limit
        [DataRow(nameof(ValidMaxLengthEmail))] // Upper valid limit
        public void Should_Accept_ValidEmail(string email)
        {
            // Arrange & Act
            var user = new User { Email = email };

            // Assert
            Assert.AreEqual(email, user.Email);
        }

        [DataTestMethod]
        [DataRow("")] // Empty string
        [DataRow(null)] // Null
        [DataRow("plainaddress")] // Missing "@" and domain
        [DataRow("@domain.com")] // Missing local part
        [DataRow("test@")] // Missing domain
        [DataRow(nameof(InvalidMaxLengthEmail))] // Exceeds max limit
        public void Should_ThrowException_ForInvalidEmail(string email)
        {
            // Arrange
            var user = new User { Email = email };

            // Act & Assert
            Assert.ThrowsException<ValidationException>(() => ValidateModel(user));
        }

        // Role Tests

        [DataTestMethod]
        [DataRow("user")] // Valid
        [DataRow("admin")] // Valid
        public void Should_Accept_ValidRole(string role)
        {
            // Arrange & Act
            var user = new User { Role = role };

            // Assert
            Assert.AreEqual(role, user.Role);
        }

        [DataTestMethod]
        [DataRow("")] // Empty string
        [DataRow(null)] // Null
        [DataRow("usr")] // Too short
        [DataRow("admins")] // Too long
        [DataRow("manager")] // Invalid value
        public void Should_ThrowException_ForInvalidRole(string role)
        {
            // Arrange
            var user = new User { Role = role };

            // Act & Assert
            Assert.ThrowsException<ValidationException>(() => ValidateModel(user));
        }

        // NotificationPreferences Tests

        [DataTestMethod]
        [DataRow("EmailOnly")] // Valid
        [DataRow("SMSOnly")] // Valid
        [DataRow("")] // Valid (optional)
        [DataRow(null)] // Valid (optional)
        public void Should_Accept_ValidNotificationPreferences(string notificationPreferences)
        {
            // Arrange & Act
            var user = new User { NotificationPreferences = notificationPreferences };

            // Assert
            Assert.AreEqual(notificationPreferences, user.NotificationPreferences);
        }

        [DataTestMethod]
        [DataRow(null)] // Null (if required in certain scenarios)
        public void Should_ThrowException_ForInvalidNotificationPreferences(string notificationPreferences)
        {
            // Arrange
            var user = new User { NotificationPreferences = notificationPreferences };

            // Act & Assert
            Assert.ThrowsException<ValidationException>(() => ValidateModel(user));
        }
    }
}