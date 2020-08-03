using System;
using System.Collections.Generic;

namespace OwlCore.Logger
{
    /// <inheritdoc/>
    public class LoggerService : ILoggerService
    {
        /// <summary>
        /// Creates a new instance of <see cref="LoggerService"/>
        /// </summary>
        public LoggerService()
        {
        }

        /// <inheritdoc/>
        public void Log(object message)
        {
            if (message == null)
                return;

            if (TimestampEnabled)
            {
                message = $"{DateTime.Now:HH:mm:ss.fff}: {message}";
            }

            StoredLogs.Add(new LogEntry(DateTime.Now, message.ToString()));

            OnLog?.Invoke(this, message.ToString());
        }

        /// <inheritdoc/>
        public event EventHandler<string>? OnLog;

        /// <inheritdoc />
        public IList<LogEntry> StoredLogs { get; set; } = new List<LogEntry>();

        /// <inheritdoc />
        public bool TimestampEnabled { get; set; }
    }
}