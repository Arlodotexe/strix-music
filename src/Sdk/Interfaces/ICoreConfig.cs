using System;

namespace StrixMusic.Sdk.Interfaces
{
    /// <summary>
    /// Provides various methods of configuring a core.
    /// </summary>
    public interface ICoreConfig
    {
        /// <summary>
        /// The services for this instance of the core.
        /// </summary>
        IServiceProvider Services { get; }
    }
}
