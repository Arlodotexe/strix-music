using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using OwlCore.Diagnostics;
using Windows.Storage;

namespace StrixMusic
{
    /// <summary>
    /// A light log formatter that provides the log  folder path and format the message.
    /// </summary>
    internal class LogFormatter
    {
        private const string LOG_RELATIVE_FOLDER_PATH = "\\Logs";

        /// <summary>
        /// The folder path that has the all the log files for the current session.
        /// </summary>
        public static string LogFolderPath => ApplicationData.Current.LocalCacheFolder.Path + LOG_RELATIVE_FOLDER_PATH;

        /// <summary>
        /// Formats the payload from <see cref="LoggerMessageEventArgs"/>.
        /// </summary>
        /// <param name="e"></param>
        /// <returns>Formatted log message</returns>
        public static string GetFormattedLogMessage(LoggerMessageEventArgs e)
            => $"{DateTime.UtcNow:O} [{e.Level}] [Thread {Thread.CurrentThread.ManagedThreadId}] L{e.CallerLineNumber} {Path.GetFileName(e.CallerFilePath)} {e.CallerMemberName} {(e.Exception is not null ? $"Exception: {e.Exception} |" : string.Empty)} {e.Message}";
    }
}
