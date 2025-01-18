namespace ArgusService.Interfaces
{
    public interface IMqttManager
    {
        Task InitializeConnectionAsync();
        Task MonitorConnectionStatusAsync();
        Task PublishMessageAsync(string topic, string payload);
        Task SubscribeToTopicAsync(string topic);
        Task UnsubscribeFromTopicAsync(string topic);
    }
}