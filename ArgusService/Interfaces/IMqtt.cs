using ArgusService.Models;

namespace ArgusService.Interfaces
{
    public interface IMqtt
    {
        Task InitializeMQTTBrokerConnectionAsync();
        Task MonitorMQTTBrokerStatusAsync();
        Task SubscribeToTopicAsync(string topic);
        Task PublishToTopicAsync(string topic, string payload);
        Task UnsubscribeFromTopicAsync(string topic);
    }
}
