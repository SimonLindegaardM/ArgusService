// File: ArgusService/Models/MqttSettings.cs

namespace ArgusService.Models
{
    public class MqttSettings
    {
        public string ClientId { get; set; }
        public string Broker { get; set; }
        public int Port { get; set; }
        public bool CleanSession { get; set; }

        // Optional: Add authentication fields if your broker requires them
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
