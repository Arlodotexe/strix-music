using System;

namespace OwlCore.Logger
{
    /// <summary>
    /// Represent a logged message with additional data
    /// </summary>
    public class LogEntry
    {
        /// <summary>
        /// Constructs a new <see cref="LogEntry"/>
        /// </summary>
        /// <param name="time"><inheritdoc /></param>
        /// <param name="message"><inheritdoc/></param>
        public LogEntry(DateTime time, string message)
        {
            Time = time;
            Message = message;
        }

        /// <summary>
        /// The time the log was created
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// The logged message
        /// </summary>
        public string Message { get; set; }
    }
}