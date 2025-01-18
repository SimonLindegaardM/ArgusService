using ArgusService.Interfaces;
using ArgusService.Repositories;

namespace ArgusService.Managers
{
    public class MqttManager : IMqttManager
    {
        private readonly IMqttRepository _mqttRepository;

        public MqttManager(IMqttRepository mqttRepository)
        {
            _mqttRepository = mqttRepository;
        }

        /// <summary>
        /// Initializes the connection to the MQTT broker.
        /// </summary>
        public async Task InitializeConnectionAsync()
        {
            await _mqttRepository.InitializeMQTTBrokerConnectionAsync();
        }

        /// <summary>
        /// Monitors the status of the MQTT broker connection.
        /// </summary>
        public async Task MonitorConnectionStatusAsync()
        {
            await _mqttRepository.MonitorMQTTBrokerStatusAsync();
        }

        /// <summary>
        /// Publishes a message to a specific MQTT topic.
        /// </summary>
        public async Task PublishMessageAsync(string topic, string payload)
        {
            if (string.IsNullOrEmpty(topic) || string.IsNullOrEmpty(payload))
            {
                throw new ArgumentException("Topic and payload cannot be null or empty.");
            }

            await _mqttRepository.PublishToTopicAsync(topic, payload);
        }

        /// <summary>
        /// Subscribes to a specific MQTT topic.
        /// </summary>
        public async Task SubscribeToTopicAsync(string topic)
        {
            if (string.IsNullOrEmpty(topic))
            {
                throw new ArgumentException("Topic cannot be null or empty.");
            }

            await _mqttRepository.SubscribeToTopicAsync(topic);
        }

        /// <summary>
        /// Unsubscribes from a specific MQTT topic.
        /// </summary>
        public async Task UnsubscribeFromTopicAsync(string topic)
        {
            if (string.IsNullOrEmpty(topic))
            {
                throw new ArgumentException("Topic cannot be null or empty.");
            }

            await _mqttRepository.UnsubscribeFromTopicAsync(topic);
        }
    }
}