using System.Collections.Generic;

namespace StrixMusic;

/// <summary>
/// Holds the current or last known health state of the application.
/// </summary>
public class ApplicationHealthState
{
    /// <summary>
    /// If true, the app was last shut down in an unhealthy state.
    /// </summary>
    public bool UnhealthyShutdown { get; set; }

    /// <summary>
    /// The reason that the shutdown was unhealthy.
    /// </summary>
    public string? UnhealthyShutdownReason { get; set; }

    /// <summary>
    /// The stack trace, if any.
    /// </summary>
    public string? StackTrace { get; set; }

    /// <summary>
    /// When an <see cref="UnhealthyShutdownReason"/> is in this list, the application will make a best effort to handle the problem without shutting down.
    /// </summary>
    public List<string> ReasonWhitelist { get; set; } = new();
}
