using ArgusService.Interfaces;
using ArgusService.Models;
using MQTTnet;
using MQTTnet.Client;

namespace ArgusService.Repositories
{
    public class MqttRepository : IMqtt
    {
        private readonly IMqttClient _mqttClient;
        private readonly MqttClientOptions _mqttOptions;

        public MqttRepository()
        {
            var factory = new MqttFactory();
            _mqttClient = factory.CreateMqttClient();

            // Configure MQTT client options
            _mqttOptions = new MqttClientOptionsBuilder()
                .WithClientId("MQTT_Client")
                .WithTcpServer("broker.example.com", 1883) // Replace with actual broker details
                .WithCleanSession()
                .Build();
        }

        /// <summary>
        /// Establishes a new connection to the MQTT broker.
        /// </summary>
        public async Task InitializeMQTTBrokerConnectionAsync()
        {
            if (!_mqttClient.IsConnected)
            {
                await _mqttClient.ConnectAsync(_mqttOptions, CancellationToken.None);
                Console.WriteLine("Connected to MQTT broker.");
            }
        }

        /// <summary>
        /// Monitors and reports the status of the MQTT broker.
        /// </summary>
        public async Task MonitorMQTTBrokerStatusAsync()
        {
            _mqttClient.ConnectedAsync += async e =>
            {
                Console.WriteLine("MQTT broker connected.");
            };

            _mqttClient.DisconnectedAsync += async e =>
            {
                Console.WriteLine("MQTT broker disconnected. Reconnecting...");
                await _mqttClient.ConnectAsync(_mqttOptions, CancellationToken.None);
            };
        }

        /// <summary>
        /// Subscribes to an MQTT topic for incoming messages.
        /// </summary>
        public async Task SubscribeToTopicAsync(string topic)
        {
            if (!_mqttClient.IsConnected)
            {
                throw new InvalidOperationException("MQTT client is not connected.");
            }

            await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder()
                .WithTopic(topic)
                .Build());

            Console.WriteLine($"Subscribed to topic: {topic}");
        }

        /// <summary>
        /// Publishes a message with a payload to a specific MQTT topic.
        /// </summary>
        public async Task PublishToTopicAsync(string topic, string payload)
        {
            if (!_mqttClient.IsConnected)
            {
                throw new InvalidOperationException("MQTT client is not connected.");
            }

            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce)
                .Build();

            await _mqttClient.PublishAsync(message);
            Console.WriteLine($"Message published to topic: {topic}");
        }

        /// <summary>
        /// Unsubscribes from a specific MQTT topic.
        /// </summary>
        public async Task UnsubscribeFromTopicAsync(string topic)
        {
            if (!_mqttClient.IsConnected)
            {
                throw new InvalidOperationException("MQTT client is not connected.");
            }

            await _mqttClient.UnsubscribeAsync(topic);
            Console.WriteLine($"Unsubscribed from topic: {topic}");
        }
    }
}
