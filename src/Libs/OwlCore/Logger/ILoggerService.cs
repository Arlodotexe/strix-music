using System;
using System.Collections.Generic;

namespace OwlCore.Logger
{
    /// <summary>
    /// A service that handles logging data
    /// </summary>
    public interface ILoggerService
    {
        /// <summary>
        /// Logs a <paramref name="message"/>
        /// </summary>
        /// <param name="message">Text to log</param>
        void Log(object message);

        /// <summary>
        /// Emitted when something new is logged
        /// </summary>
        event EventHandler<string> OnLog;

        /// <summary>
        /// The entire internal log
        /// </summary>
        IList<LogEntry> StoredLogs { get; }

        /// <summary>
        /// If true, the emitted log will contain a timestamp
        /// </summary>
        bool TimestampEnabled { get; set; }
    }
}
