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
    public class LocationModelTests
    {
        private const string LongString128 = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"; // 128 'a's
        private const string LongString129 = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"; // 129 'a's

        private Location CreateLocation(string trackerId, float latitude, float longitude, DateTime timestamp)
        {
            return new Location
            {
                TrackerId = trackerId,
                Latitude = latitude,
                Longitude = longitude,
                Timestamp = timestamp
            };
        }

        private bool TryValidateModel(Location location, out ICollection<ValidationResult> results)
        {
            var context = new ValidationContext(location, serviceProvider: null, items: null);
            results = new List<ValidationResult>();
            return Validator.TryValidateObject(location, context, results, true);
        }

        // TrackerId Tests
        [DataTestMethod]
        [DataRow("Tracker123")] // Valid
        [DataRow("a")] // Lower valid limit
        [DataRow(LongString128)] // Upper valid limit
        public void Should_Accept_ValidTrackerId(string trackerId)
        {
            var location = CreateLocation(trackerId, 0.0f, 0.0f, DateTime.UtcNow);
            var result = TryValidateModel(location, out var validationResults);
            Assert.IsTrue(result);
            Assert.AreEqual(0, validationResults.Count);
        }

        [DataTestMethod]
        [DataRow("")] // Empty string
        [DataRow(null)] // Null
        [DataRow(LongString129)] // Exceeds max limit
        public void Should_ThrowException_ForInvalidTrackerId(string trackerId)
        {
            var location = CreateLocation(trackerId, 0.0f, 0.0f, DateTime.UtcNow);
            var result = TryValidateModel(location, out var validationResults);
            Assert.IsFalse(result);
            Assert.IsTrue(validationResults.Count > 0);
        }

        // Latitude Tests
        [DataTestMethod]
        [DataRow(-90)] // Lower valid limit
        [DataRow(0)] 
        [DataRow(90)] // Upper valid limit
        public void Should_Accept_ValidLatitude(float latitude)
        {
            var location = CreateLocation("Tracker123", latitude, 0.0f, DateTime.UtcNow);
            var result = TryValidateModel(location, out var validationResults);
            Assert.IsTrue(result);
            Assert.AreEqual(0, validationResults.Count);
        }

        [DataTestMethod]
        [DataRow(-91)] // Below lower limit
        [DataRow(91)] // Above upper limit
        public void Should_ThrowException_ForInvalidLatitude(float latitude)
        {
            var location = CreateLocation("Tracker123", latitude, 0.0f, DateTime.UtcNow);
            var result = TryValidateModel(location, out var validationResults);
            Assert.IsFalse(result);
            Assert.IsTrue(validationResults.Count > 0);
        }

        // Longitude Tests
        [DataTestMethod]
        [DataRow(-180)] // Lower valid limit
        [DataRow(0)] 
        [DataRow(180)] // Upper valid limit
        public void Should_Accept_ValidLongitude(float longitude)
        {
            var location = CreateLocation("Tracker123", 0.0f, longitude, DateTime.UtcNow);
            var result = TryValidateModel(location, out var validationResults);
            Assert.IsTrue(result);
            Assert.AreEqual(0, validationResults.Count);
        }

        [DataTestMethod]
        [DataRow(-181)] // Below lower limit
        [DataRow(181)] // Above upper limit
        public void Should_ThrowException_ForInvalidLongitude(float longitude)
        {
            var location = CreateLocation("Tracker123", 0.0f, longitude, DateTime.UtcNow);
            var result = TryValidateModel(location, out var validationResults);
            Assert.IsFalse(result);
            Assert.IsTrue(validationResults.Count > 0);
        }

        // Timestamp Tests
        [TestMethod]
        public void Should_Accept_ValidTimestamp()
        {
            var now = DateTime.UtcNow;
            var location = CreateLocation("Tracker123", 0.0f, 0.0f, now);
            var result = TryValidateModel(location, out var validationResults);
            Assert.IsTrue(result);
            Assert.AreEqual(0, validationResults.Count);
        }
    }
}
