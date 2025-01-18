using ArgusService.Models;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArgusService_UnitTest
{
    [TestClass]
    public class MotionModelTests
    {
        private const string LongString128 = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"; // 128 'a's

        private const string LongString129 = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"; // 129 'a's

        private Motion CreateMotion(string trackerId, bool motionDetected, DateTime timestamp)
        {
            return new Motion
            {
                TrackerId = trackerId,
                MotionDetected = motionDetected,
                Timestamp = timestamp,
            };
        }

        private bool TryValidateModel(Motion motion, out ICollection<ValidationResult> results)
        {
            var context = new ValidationContext(motion, serviceProvider: null, items: null);
            results = new List<ValidationResult>();
            return Validator.TryValidateObject(motion, context, results, true);
        }

        // TrackerId Tests
        [DataTestMethod]
        [DataRow("Tracker123")] // Valid
        [DataRow("a")] // Lower valid limit
        [DataRow(LongString128)] // Upper valid limit
        public void Should_Accept_ValidTrackerId(string trackerId)
        {
            var motion = CreateMotion(trackerId, false, DateTime.Now);
            var result = TryValidateModel(motion, out var validationResults);
            Assert.IsTrue(result);
            Assert.AreEqual(0, validationResults.Count);
        }

        [DataTestMethod]
        [DataRow("")] // Empty string
        [DataRow(null)] // Null
        [DataRow(LongString129)] // Exceeds max limit
        public void Should_ThrowException_ForInvalidTrackerId(string trackerId)
        {
            var motion = CreateMotion(trackerId, false, DateTime.Now);
            var result = TryValidateModel(motion, out var validationResults);
            Assert.IsFalse(result);
            Assert.IsTrue(validationResults.Count > 0);

        }

        // MotionDetected Tests
        [DataTestMethod]
        [DataRow(true)] // Motion detected
        [DataRow(false)] // No motion detected
        public void Should_Accept_ValidMotionDetected(bool motionDetected)
        {
            var motion = CreateMotion("tracker1", motionDetected, DateTime.Now);
            var result = TryValidateModel(motion, out var validationResults);
            Assert.IsTrue(result);
            Assert.AreEqual(0, validationResults.Count);

        }

        // Timestamp Tests
        [TestMethod]
        public void Should_Accept_ValidTimestamp()
        {
            // Arrange
            var now = DateTime.UtcNow;

            // Act
            var motion = new Motion { Timestamp = now };

            // Assert
            Assert.AreEqual(now, motion.Timestamp);
        }
    }
}
