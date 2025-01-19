// File: ArgusService/Controllers/MqttController.cs

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ArgusService.DTOs;
using ArgusService.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using AutoMapper;
using ArgusService.Models;

namespace ArgusService.Controllers
{
    /// <summary>
    /// Controller for managing MQTT operations.
    /// </summary>
    [ApiController]
    [Route("api/mqtt")]
    public class MqttController : ControllerBase
    {
        private readonly IMqttRepository _mqttRepository;
        private readonly ILogger<MqttController> _logger;
        private readonly IMapper _mapper;

        public MqttController(IMqttRepository mqttRepository, ILogger<MqttController> logger, IMapper mapper)
        {
            _mqttRepository = mqttRepository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Publishes an MQTT message.
        /// </summary>
        /// <param name="request">The MQTT message details.</param>
        /// <returns>Result of the publish operation.</returns>
        /// <response code="200">MQTT message published successfully.</response>
        /// <response code="500">If an unexpected error occurs.</response>
        [HttpPost("publish")]
       // [Authorize(Roles = "admin,user")]
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
