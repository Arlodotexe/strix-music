using System;
using System.Collections.Generic;
using OwlCore.AbstractUI;

namespace StrixMusic.Sdk.Core.Data
{
    /// <summary>
    /// Provides various methods of configuring a core.
    /// </summary>
    public interface ICoreConfig : ICoreMember
    {
        /// <summary>
        /// The services for this instance of the core.
        /// </summary>
        IServiceProvider? Services { get; }

        /// <summary>
        /// Abstract UI elements that will be presented to the user for Settings, About, Legal notices, Donation links, etc.
        /// </summary>
        IReadOnlyList<AbstractUIElementGroup> CoreDataUIElements { get; }

        /// <summary>
        /// A local path or url pointing to a SVG file containing the logo for this core.
        /// </summary>
        Uri LogoSvgUrl { get; }
    }
}
