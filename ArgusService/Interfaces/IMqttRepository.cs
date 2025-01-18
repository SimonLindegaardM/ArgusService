// File: ArgusService/Interfaces/IMqttRepository.cs
using ArgusService.Models;

namespace ArgusService.Interfaces
{
    public interface IMqttRepository
    {
        Task InitializeMQTTBrokerConnectionAsync();
        Task MonitorMQTTBrokerStatusAsync();
        Task SubscribeToTopicAsync(string topic);
        Task PublishToTopicAsync(string topic, string payload);
        Task UnsubscribeFromTopicAsync(string topic);
        Task PublishMessageAsync(MqttMessage message);
    }
}
