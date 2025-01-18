// File: ArgusService/Controllers/MqttController.cs

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ArgusService.DTOs;
using ArgusService.Interfaces;   // IMqttRepository interface
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ArgusService.Controllers
{
    [ApiController]
    [Route("api/mqtt")]
    public class MqttController : ControllerBase
    {
        private readonly IMqttRepository _mqttRepository;
        private readonly ILogger<MqttController> _logger;

        public MqttController(IMqttRepository mqttRepository, ILogger<MqttController> logger)
        {
            _mqttRepository = mqttRepository;
            _logger = logger;
        }

        /// <summary>
        /// Publishes an MQTT message.
        /// Example body:
        /// {
        ///   "trackerId": "Tracker001",
        ///   "topicType": "telemetry",
        ///   "payload": { "data": "sample" }
        /// }
        /// </summary>
        [HttpPost("publish")]
        [Authorize(Roles = "admin,user")]
        public async Task<IActionResult> PublishMessage([FromBody] MqttPublishRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid MqttPublishRequestDto received.");
                return BadRequest(ModelState);
            }

            try
            {
                await _mqttRepository.PublishMessageAsync(request);
                _logger.LogInformation("Published MQTT message to Tracker '{TrackerId}' on topic '{TopicType}'.", request.TrackerId, request.TopicType);
                return Ok(new { Message = "MQTT message published successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing MQTT message for Tracker '{TrackerId}'.", request.TrackerId);
                return StatusCode(500, new { Message = "An error occurred while publishing the MQTT message." });
            }
        }
    }
}
