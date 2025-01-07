using ArgusService.Interfaces;
using ArgusService.Managers;
using Microsoft.AspNetCore.Mvc;

namespace ArgusService.Controllers
{
    [ApiController]
    [Route("api/mqtt")]
    public class MQTTController : ControllerBase
    {
        private readonly MqttManager _mqttManager;

        public MQTTController(MqttManager mqttManager)
        {
            _mqttManager = mqttManager;
        }

        /// <summary>
        /// Initializes the connection to the MQTT broker.
        /// </summary>
        [HttpPost("initialize")]
        public async Task<IActionResult> InitializeConnection()
        {
            try
            {
                await _mqttManager.InitializeConnectionAsync();
                return Ok(new { Message = "MQTT connection initialized successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Monitors the MQTT broker connection status.
        /// </summary>
        [HttpGet("status")]
        public async Task<IActionResult> MonitorConnectionStatus()
        {
            try
            {
                await _mqttManager.MonitorConnectionStatusAsync();
                return Ok(new { Message = "MQTT connection status monitored successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Publishes a message to a specific MQTT topic.
        /// </summary>
        [HttpPost("publish")]
        public async Task<IActionResult> PublishMessage([FromBody] PublishMessageRequest request)
        {
            try
            {
                await _mqttManager.PublishMessageAsync(request.Topic, request.Payload);
                return Ok(new { Message = "Message published successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Subscribes to a specific MQTT topic.
        /// </summary>
        [HttpPost("subscribe")]
        public async Task<IActionResult> SubscribeToTopic([FromBody] TopicRequest request)
        {
            try
            {
                await _mqttManager.SubscribeToTopicAsync(request.Topic);
                return Ok(new { Message = "Subscribed to topic successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Unsubscribes from a specific MQTT topic.
        /// </summary>
        [HttpPost("unsubscribe")]
        public async Task<IActionResult> UnsubscribeFromTopic([FromBody] TopicRequest request)
        {
            try
            {
                await _mqttManager.UnsubscribeFromTopicAsync(request.Topic);
                return Ok(new { Message = "Unsubscribed from topic successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }

    public class PublishMessageRequest
    {
        public string Topic { get; set; }
        public string Payload { get; set; }
    }

    public class TopicRequest
    {
        public string Topic { get; set; }
    }
}
