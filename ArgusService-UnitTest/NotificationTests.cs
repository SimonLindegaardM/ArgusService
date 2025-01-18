using ArgusService.Interfaces;
using ArgusService.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArgusService_UnitTest
{
    [TestClass]
    public class NotificationTests
    {
        private const string LongString128 = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"; // 128 'a's
        private const string LongString129 = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"; // 129 'a's
        private const string LongString500 = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"; // 500 'a's
        private const string LongString501 = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"; // 501 'a's

        private Notification CreateNotification(string notificationId, string type, string message, DateTime timestamp)
        {
            return new Notification
            {
                NotificationId = notificationId,
                Type = type,
                Message = message,
                Timestamp = timestamp,
            };
        }

        private bool TryValidateModel(Notification notification, out ICollection<ValidationResult> results)
        {
            var context = new ValidationContext(notification, serviceProvider: null, items: null);
            results = new List<ValidationResult>();
            return Validator.TryValidateObject(notification, context, results, true);
        }

        // NotificationId Tests
        [DataTestMethod]
        [DataRow("Notif123")] // Valid
        [DataRow("a")] // Lower valid limit
        [DataRow(LongString128)] // Upper valid limit
        public void Should_Accept_ValidNotificationId(string notificationId)
        {
            var notification = CreateNotification(notificationId, "motion", "Valid message", DateTime.UtcNow);
            var result = TryValidateModel(notification, out var validationResults);
            Assert.IsTrue(result);
            Assert.AreEqual(0, validationResults.Count);
        }

        [DataTestMethod]
        [DataRow("")] // Empty string
        [DataRow(null)] // Null
        [DataRow(LongString129)] // Exceeds max limit
        public void Should_ThrowException_ForInvalidNotificationId(string notificationId)
        {
            var notification = CreateNotification(notificationId, "motion", "Valid message", DateTime.UtcNow);
            var result = TryValidateModel(notification, out var validationResults);
            Assert.IsFalse(result);
            Assert.IsTrue(validationResults.Count > 0);
        }

        // Type Tests
        [DataTestMethod]
        [DataRow("motion")] // Valid
        [DataRow("lockState")] // Valid
        public void Should_Accept_ValidType(string type)
        {
            var notification = CreateNotification("Notif123", type, "Valid message", DateTime.UtcNow);
            var result = TryValidateModel(notification, out var validationResults);
            Assert.IsTrue(result);
            Assert.AreEqual(0, validationResults.Count);
        }

        [DataTestMethod]
        [DataRow("")] // Empty string
        [DataRow(null)] // Null
        [DataRow("invalidType")] // Invalid
        public void Should_ThrowException_ForInvalidType(string type)
        {
            var notification = CreateNotification("Notif123", type, "Valid message", DateTime.UtcNow);
            var result = TryValidateModel(notification, out var validationResults);
            Assert.IsFalse(result);
            Assert.IsTrue(validationResults.Count > 0);
        }

        // Message Tests
        [DataTestMethod]
        [DataRow("This is a valid message.")] // Valid
        [DataRow(LongString500)] // Upper valid limit
        public void Should_Accept_ValidMessage(string message)
        {
            var notification = CreateNotification("Notif123", "motion", message, DateTime.UtcNow);
            var result = TryValidateModel(notification, out var validationResults);
            Assert.IsTrue(result);
            Assert.AreEqual(0, validationResults.Count);
        }

        [DataTestMethod]
        [DataRow("")] // Empty string
        [DataRow(null)] // Null
        [DataRow(LongString501)] // Exceeds max limit
        public void Should_ThrowException_ForInvalidMessage(string message)
        {
            var notification = CreateNotification("Notif123", "motion", message, DateTime.UtcNow);
            var result = TryValidateModel(notification, out var validationResults);
            Assert.IsFalse(result);
            Assert.IsTrue(validationResults.Count > 0);
        }

        // Timestamp Tests
        [TestMethod]
        public void Should_Accept_ValidTimestamp()
        {
            var now = DateTime.UtcNow;
            var notification = CreateNotification("Notif123", "motion", "Valid message", now);
            var result = TryValidateModel(notification, out var validationResults);
            Assert.IsTrue(result);
            Assert.AreEqual(0, validationResults.Count);
        }
    }
}
