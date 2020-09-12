using System;
using System.Collections.Generic;
using StrixMusic.Sdk.AbstractUI;

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

        /// <summary>
        /// Abstract UI elements that will be presented to the user for Settings, About, Legal notices, Donation links, etc.
        /// </summary>
        IReadOnlyList<AbstractUIElementGroup> CoreDataUIElements { get; }
    }
}
