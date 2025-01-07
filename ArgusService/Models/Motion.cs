namespace ArgusService.Models
{
    public class Motion
    {
        /// <summary>
        /// ID of the associated tracker.
        /// </summary>
        public string TrackerId { get; set; }

        /// <summary>
        /// Motion detected status.
        /// </summary>
        public bool MotionDetected { get; set; }

        /// <summary>
        /// Time when the motion was detected.
        /// </summary>
        public DateTime Timestamp { get; set; }
    }
}
