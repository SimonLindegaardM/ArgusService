// File: ArgusService/Models/MqttMessage.cs

using System;

namespace ArgusService.Models
{
    /// <summary>
    /// Model representing an MQTT message.
    /// </summary>
    public class MqttMessage
    {
        public string TrackerId { get; set; }
        public string TopicType { get; set; }
        public object Payload { get; set; }
    }
}
