using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace StrixMusic.Sdk.Messages
{
    /// <summary>
    /// Used to relay the contents of a log message via the messenger pattern.
    /// </summary>
    public class LogMessage
    {
        /// <summary>
        /// Creates a new instance of <see cref="LogMessage"/>.
        /// </summary>
        /// <param name="message">The contents of the message that was logged.</param>
        /// <param name="level">The logging level for this message.</param>
        public LogMessage(string message, LogLevel level)
        {
            Message = message;
            Level = level;
        }

        /// <summary>
        /// Creates a new instance of <see cref="LogMessage"/>.
        /// </summary>
        /// <param name="message">The contents of the message that was logged.</param>
        /// <param name="level">The logging level for this message.</param>
        /// <param name="stackTrace">The stack trace, if available.</param>
        public LogMessage(string message, LogLevel level, StackTrace stackTrace)
            : this(message, level)
        {
            StackTrace = stackTrace;
        }

        /// <summary>
        /// The logging level for this message.
        /// </summary>
        public LogLevel Level { get; set; }

        /// <summary>
        /// The contents of the log message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The stack trace, if available.
        /// </summary>
        public StackTrace? StackTrace { get; set; }
    }
}
