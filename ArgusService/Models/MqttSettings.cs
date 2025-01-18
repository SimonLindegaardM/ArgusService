// File: ArgusService/Models/MqttSettings.cs

namespace ArgusService.Models
{
    /// <summary>
    /// Configuration settings for MQTT.
    /// </summary>
    public class MqttSettings
    {
        public string ClientId { get; set; }
        public string Broker { get; set; }
        public int Port { get; set; }
        public bool CleanSession { get; set; }
        public string Username { get; set; } // Optional
        public string Password { get; set; } // Optional
    }
}
