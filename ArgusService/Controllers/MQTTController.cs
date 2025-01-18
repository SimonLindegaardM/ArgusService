// File: ArgusService/Controllers/MqttController.cs

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ArgusService.DTOs;
using ArgusService.Interfaces;   // IMqttRepository interface
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using AutoMapper; // Add this
using ArgusService.Models; // Ensure this is included for the MqttMessage model

namespace ArgusService.Controllers
{
    [ApiController]
    [Route("api/mqtt")]
    public class MqttController : ControllerBase
    {
        private readonly IMqttRepository _mqttRepository;
        private readonly ILogger<MqttController> _logger;
        private readonly IMapper _mapper; // Inject IMapper

        public MqttController(IMqttRepository mqttRepository, ILogger<MqttController> logger, IMapper mapper)
        {
            _mqttRepository = mqttRepository;
            _logger = logger;
            _mapper = mapper; // Assign IMapper
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
                // Map DTO to Model
                var mqttMessage = _mapper.Map<MqttMessage>(request);

                await _mqttRepository.PublishMessageAsync(mqttMessage);
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
