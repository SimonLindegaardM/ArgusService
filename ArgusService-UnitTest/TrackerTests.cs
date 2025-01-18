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
    public class TrackerTests
    {
        private const string LongString128 = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"; // 128 'a's
        private const string LongString129 = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"; // 129 'a's
        private const string LongString100 = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"; // 100 'a's
        private const string LongString101 = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"; // 101 'a's

        private Tracker CreateTracker(string trackerId, string firebaseUID, string email, string mqttUsername, string mqttPassword, string brokerUrl, int port, string lockState, DateTime lastUpdated)
        {
            return new Tracker
            {
                TrackerId = trackerId,
                FirebaseUID = firebaseUID,
                Email = email,
                MqttUsername = mqttUsername,
                MqttPassword = mqttPassword,
                BrokerUrl = brokerUrl,
                Port = port,
                LockState = lockState,
                LastUpdated = lastUpdated
            };
        }

        private bool TryValidateModel(Tracker tracker, out ICollection<ValidationResult> results)
        {
            var context = new ValidationContext(tracker, serviceProvider: null, items: null);
            results = new List<ValidationResult>();
            return Validator.TryValidateObject(tracker, context, results, true);
        }

        // TrackerId Tests
        [DataTestMethod]
        [DataRow("Tracker123")] // Valid
        [DataRow("a")] // Lower valid limit
        [DataRow(LongString128)] // Upper valid limit
        public void Should_Accept_ValidTrackerId(string trackerId)
        {
            var tracker = CreateTracker(trackerId, "FirebaseUID123", "test@example.com", "mqttUser", "mqttPass", "http://broker.com", 1883, "locked", DateTime.UtcNow);
            var result = TryValidateModel(tracker, out var validationResults);
            Assert.IsTrue(result);
            Assert.AreEqual(0, validationResults.Count);
        }

        [DataTestMethod]
        [DataRow("")] // Empty string
        [DataRow(null)] // Null
        [DataRow(LongString129)] // Exceeds max limit
        public void Should_ThrowException_ForInvalidTrackerId(string trackerId)
        {
            var tracker = CreateTracker(trackerId, "FirebaseUID123", "test@example.com", "mqttUser", "mqttPass", "http://broker.com", 1883, "locked", DateTime.UtcNow);
            var result = TryValidateModel(tracker, out var validationResults);
            Assert.IsFalse(result);
            Assert.IsTrue(validationResults.Count > 0);
        }

        // FirebaseUID Tests
        [DataTestMethod]
        [DataRow("FirebaseUID123")] // Valid
        [DataRow("a")] // Lower valid limit
        [DataRow(LongString128)] // Upper valid limit
        public void Should_Accept_ValidFirebaseUID(string firebaseUID)
        {
            var tracker = CreateTracker("Tracker123", firebaseUID, "test@example.com", "mqttUser", "mqttPass", "http://broker.com", 1883, "locked", DateTime.UtcNow);
            var result = TryValidateModel(tracker, out var validationResults);
            Assert.IsTrue(result);
            Assert.AreEqual(0, validationResults.Count);
        }

        [DataTestMethod]
        [DataRow("")] // Empty string
        [DataRow(null)] // Null
        [DataRow(LongString129)] // Exceeds max limit
        public void Should_ThrowException_ForInvalidFirebaseUID(string firebaseUID)
        {
            var tracker = CreateTracker("Tracker123", firebaseUID, "test@example.com", "mqttUser", "mqttPass", "http://broker.com", 1883, "locked", DateTime.UtcNow);
            var result = TryValidateModel(tracker, out var validationResults);
            Assert.IsFalse(result);
            Assert.IsTrue(validationResults.Count > 0);
        }

        // Email Tests
        [DataTestMethod]
        [DataRow("test@example.com")] // Valid
        [DataRow("a@b.com")] // Lower valid limit
        public void Should_Accept_ValidEmail(string email)
        {
            var tracker = CreateTracker("Tracker123", "FirebaseUID123", email, "mqttUser", "mqttPass", "http://broker.com", 1883, "locked", DateTime.UtcNow);
            var result = TryValidateModel(tracker, out var validationResults);
            Assert.IsTrue(result);
            Assert.AreEqual(0, validationResults.Count);
        }

        [DataTestMethod]
        [DataRow("")] // Empty string
        [DataRow(null)] // Null
        [DataRow("plainaddress")] // Invalid format
        public void Should_ThrowException_ForInvalidEmail(string email)
        {
            var tracker = CreateTracker("Tracker123", "FirebaseUID123", email, "mqttUser", "mqttPass", "http://broker.com", 1883, "locked", DateTime.UtcNow);
            var result = TryValidateModel(tracker, out var validationResults);
            Assert.IsFalse(result);
            Assert.IsTrue(validationResults.Count > 0);
        }

        // Port Tests
        [DataTestMethod]
        [DataRow(1)] // Lower valid limit
        [DataRow(1883)] // Common MQTT port
        [DataRow(65535)] // Upper valid limit
        public void Should_Accept_ValidPort(int port)
        {
            var tracker = CreateTracker("Tracker123", "FirebaseUID123", "test@example.com", "mqttUser", "mqttPass", "http://broker.com", port, "locked", DateTime.UtcNow);
            var result = TryValidateModel(tracker, out var validationResults);
            Assert.IsTrue(result);
            Assert.AreEqual(0, validationResults.Count);
        }

        [DataTestMethod]
        [DataRow(0)] // Below lower limit
        [DataRow(65536)] // Above upper limit
        public void Should_ThrowException_ForInvalidPort(int port)
        {
            var tracker = CreateTracker("Tracker123", "FirebaseUID123", "test@example.com", "mqttUser", "mqttPass", "http://broker.com", port, "locked", DateTime.UtcNow);
            var result = TryValidateModel(tracker, out var validationResults);
            Assert.IsFalse(result);
            Assert.IsTrue(validationResults.Count > 0);
        }
    }
}
