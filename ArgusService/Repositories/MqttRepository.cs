// File: ArgusService/Repositories/MqttRepository.cs

using ArgusService.Interfaces;
using ArgusService.Models;
using Microsoft.Extensions.Options;
using MQTTnet;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ArgusService.Repositories
{
    public class MqttRepository : IMqttRepository
    {
        private readonly IMqttClient _mqttClient;
        private readonly MqttClientOptions _mqttOptions;
        private readonly ILogger<MqttRepository> _logger;

        public MqttRepository(IMqttClient mqttClient, IOptions<MqttSettings> mqttSettings, ILogger<MqttRepository> logger)
        {
            _mqttClient = mqttClient ?? throw new ArgumentNullException(nameof(mqttClient));
            var settings = mqttSettings?.Value ?? throw new ArgumentNullException(nameof(mqttSettings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            var mqttOptionsBuilder = new MqttClientOptionsBuilder()
                .WithClientId(settings.ClientId)
                .WithTcpServer(settings.Broker, settings.Port)
                .WithCleanSession(settings.CleanSession);

            // Add authentication if credentials are provided
            if (!string.IsNullOrEmpty(settings.Username) && !string.IsNullOrEmpty(settings.Password))
            {
                mqttOptionsBuilder = mqttOptionsBuilder.WithCredentials(settings.Username, settings.Password);
            }

            _mqttOptions = mqttOptionsBuilder.Build();
        }

        /// <summary>
        /// Establishes a new connection to the MQTT broker.
        /// </summary>
        public async Task InitializeMQTTBrokerConnectionAsync()
        {
            if (!_mqttClient.IsConnected)
            {
                try
                {
                    await _mqttClient.ConnectAsync(_mqttOptions, CancellationToken.None);
                    _logger.LogInformation("Connected to MQTT broker.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to connect to MQTT broker.");
                    throw;
                }
            }
            else
            {
                _logger.LogInformation("MQTT client is already connected.");
            }
        }

        /// <summary>
        /// Monitors and reports the status of the MQTT broker.
        /// </summary>
        public async Task MonitorMQTTBrokerStatusAsync()
        {
            _mqttClient.ConnectedAsync += async e =>
            {
                _logger.LogInformation("MQTT broker connected.");
            };

            _mqttClient.DisconnectedAsync += async e =>
            {
                _logger.LogWarning("MQTT broker disconnected. Attempting to reconnect...");
                try
                {
                    await _mqttClient.ConnectAsync(_mqttOptions, CancellationToken.None);
                    _logger.LogInformation("Reconnected to MQTT broker.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to reconnect to MQTT broker.");
                }
            };
        }

        /// <summary>
        /// Subscribes to an MQTT topic for incoming messages.
        /// </summary>
        public async Task SubscribeToTopicAsync(string topic)
        {
            if (string.IsNullOrEmpty(topic))
                throw new ArgumentException("Topic cannot be null or empty.", nameof(topic));

            if (!_mqttClient.IsConnected)
            {
                _logger.LogError("Cannot subscribe to topic '{Topic}' because MQTT client is not connected.", topic);
                throw new InvalidOperationException("MQTT client is not connected.");
            }

            try
            {
                await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder()
                    .WithTopic(topic)
                    .Build());

                _logger.LogInformation("Subscribed to topic: {Topic}", topic);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to subscribe to topic: {Topic}", topic);
                throw;
            }
        }

        /// <summary>
        /// Publishes a message with a payload to a specific MQTT topic.
        /// </summary>
        public async Task PublishToTopicAsync(string topic, string payload)
        {
            if (string.IsNullOrEmpty(topic))
                throw new ArgumentException("Topic cannot be null or empty.", nameof(topic));

            if (string.IsNullOrEmpty(payload))
                throw new ArgumentException("Payload cannot be null or empty.", nameof(payload));

            if (!_mqttClient.IsConnected)
            {
                _logger.LogError("Cannot publish to topic '{Topic}' because MQTT client is not connected.", topic);
                throw new InvalidOperationException("MQTT client is not connected.");
            }

            try
            {
                var message = new MqttApplicationMessageBuilder()
                    .WithTopic(topic)
                    .WithPayload(payload)
                    .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce)
                    .WithRetainFlag(false)
                    .Build();

                await _mqttClient.PublishAsync(message);
                _logger.LogInformation("Message published to topic: {Topic}", topic);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish message to topic: {Topic}", topic);
                throw;
            }
        }

        /// <summary>
        /// Unsubscribes from a specific MQTT topic.
        /// </summary>
        public async Task UnsubscribeFromTopicAsync(string topic)
        {
            if (string.IsNullOrEmpty(topic))
                throw new ArgumentException("Topic cannot be null or empty.", nameof(topic));

            if (!_mqttClient.IsConnected)
            {
                _logger.LogError("Cannot unsubscribe from topic '{Topic}' because MQTT client is not connected.", topic);
                throw new InvalidOperationException("MQTT client is not connected.");
            }

            try
            {
                await _mqttClient.UnsubscribeAsync(topic);
                _logger.LogInformation("Unsubscribed from topic: {Topic}", topic);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to unsubscribe from topic: {Topic}", topic);
                throw;
            }
        }

        /// <summary>
        /// Publishes an MQTT message.
        /// </summary>
        /// <param name="message">The MQTT message to publish.</param>
        public async Task PublishMessageAsync(MqttMessage message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (string.IsNullOrEmpty(message.TrackerId))
                throw new ArgumentException("TrackerId cannot be null or empty.", nameof(message.TrackerId));

            if (string.IsNullOrEmpty(message.TopicType))
                throw new ArgumentException("TopicType cannot be null or empty.", nameof(message.TopicType));

            if (!_mqttClient.IsConnected)
            {
                _logger.LogError("Cannot publish MQTT message because MQTT client is not connected.");
                throw new InvalidOperationException("MQTT client is not connected.");
            }

            try
            {
                var topic = $"{message.TrackerId}/{message.TopicType}";
                var payload = System.Text.Json.JsonSerializer.Serialize(message.Payload);

                var mqttMessagePayload = new MqttApplicationMessageBuilder()
                    .WithTopic(topic)
                    .WithPayload(payload)
                    .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce)
                    .WithRetainFlag(false)
                    .Build();

                await _mqttClient.PublishAsync(mqttMessagePayload);
                _logger.LogInformation("Published MQTT message to topic: {Topic}", topic);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish MQTT message to topic: {Topic}", $"{message.TrackerId}/{message.TopicType}");
                throw;
            }
        }
    }
}
