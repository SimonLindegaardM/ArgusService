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
    public class LockTests
    {
        private const string LongString128 = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"; // 128 'a's
        private const string LongString129 = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"; // 129 'a's

        private Lock CreateLock(string deviceId, string firebaseUID, string email, string attachedTrackerId, string status, DateTime lastUpdated)
        {
            return new Lock
            {
                DeviceId = deviceId,
                FirebaseUID = firebaseUID,
                Email = email,
                AttachedTrackerId = attachedTrackerId,
                Status = status,
                LastUpdated = lastUpdated
            };
        }

        private bool TryValidateModel(Lock lockModel, out ICollection<ValidationResult> results)
        {
            var context = new ValidationContext(lockModel, serviceProvider: null, items: null);
            results = new List<ValidationResult>();
            return Validator.TryValidateObject(lockModel, context, results, true);
        }

        // DeviceId Tests
        [DataTestMethod]
        [DataRow("Device123")] // Valid
        [DataRow("a")] // Lower valid limit
        [DataRow(LongString128)] // Upper valid limit
        public void Should_Accept_ValidDeviceId(string deviceId)
        {
            var lockModel = CreateLock(deviceId, "FirebaseUID123", "test@example.com", "Tracker123", "locked", DateTime.UtcNow);
            var result = TryValidateModel(lockModel, out var validationResults);
            Assert.IsTrue(result);
            Assert.AreEqual(0, validationResults.Count);
        }

        [DataTestMethod]
        [DataRow("")] // Empty string
        [DataRow(null)] // Null
        [DataRow(LongString129)] // Exceeds max limit
        public void Should_ThrowException_ForInvalidDeviceId(string deviceId)
        {
            var lockModel = CreateLock(deviceId, "FirebaseUID123", "test@example.com", "Tracker123", "locked", DateTime.UtcNow);
            var result = TryValidateModel(lockModel, out var validationResults);
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
            var lockModel = CreateLock("Device123", firebaseUID, "test@example.com", "Tracker123", "locked", DateTime.UtcNow);
            var result = TryValidateModel(lockModel, out var validationResults);
            Assert.IsTrue(result);
            Assert.AreEqual(0, validationResults.Count);
        }

        [DataTestMethod]
        [DataRow("")] // Empty string
        [DataRow(null)] // Null
        [DataRow(LongString129)] // Exceeds max limit
        public void Should_ThrowException_ForInvalidFirebaseUID(string firebaseUID)
        {
            var lockModel = CreateLock("Device123", firebaseUID, "test@example.com", "Tracker123", "locked", DateTime.UtcNow);
            var result = TryValidateModel(lockModel, out var validationResults);
            Assert.IsFalse(result);
            Assert.IsTrue(validationResults.Count > 0);
        }

        // Email Tests
        [DataTestMethod]
        [DataRow("test@example.com")] // Valid
        [DataRow("a@b.com")] // Lower valid limit
        public void Should_Accept_ValidEmail(string email)
        {
            var lockModel = CreateLock("Device123", "FirebaseUID123", email, "Tracker123", "locked", DateTime.UtcNow);
            var result = TryValidateModel(lockModel, out var validationResults);
            Assert.IsTrue(result);
            Assert.AreEqual(0, validationResults.Count);
        }

        [DataTestMethod]
        [DataRow("")] // Empty string
        [DataRow(null)] // Null
        [DataRow("plainaddress")] // Invalid format
        public void Should_ThrowException_ForInvalidEmail(string email)
        {
            var lockModel = CreateLock("Device123", "FirebaseUID123", email, "Tracker123", "locked", DateTime.UtcNow);
            var result = TryValidateModel(lockModel, out var validationResults);
            Assert.IsFalse(result);
            Assert.IsTrue(validationResults.Count > 0);
        }

        // Status Tests
        [DataTestMethod]
        [DataRow("locked")] // Valid
        [DataRow("unlocked")] // Valid
        public void Should_Accept_ValidStatus(string status)
        {
            var lockModel = CreateLock("Device123", "FirebaseUID123", "test@example.com", "Tracker123", status, DateTime.UtcNow);
            var result = TryValidateModel(lockModel, out var validationResults);
            Assert.IsTrue(result);
            Assert.AreEqual(0, validationResults.Count);
        }

        [DataTestMethod]
        [DataRow("")] // Empty string
        [DataRow(null)] // Null
        [DataRow("test")] // Invalid
        public void Should_ThrowException_ForInvalidStatus(string status)
        {
            var lockModel = CreateLock("Device123", "FirebaseUID123", "test@example.com", "Tracker123", status, DateTime.UtcNow);
            var result = TryValidateModel(lockModel, out var validationResults);
            Assert.IsFalse(result);
            Assert.IsTrue(validationResults.Count > 0);
        }

        // LastUpdated Tests
        [TestMethod]
        public void Should_Accept_ValidLastUpdated()
        {
            var now = DateTime.UtcNow;
            var lockModel = CreateLock("Device123", "FirebaseUID123", "test@example.com", "Tracker123", "locked", now);
            var result = TryValidateModel(lockModel, out var validationResults);
            Assert.IsTrue(result);
            Assert.AreEqual(0, validationResults.Count);
        }
    }
}
