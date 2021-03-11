using System;
using System.Collections.Generic;
using OwlCore.AbstractUI;
using OwlCore.AbstractUI.Models;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Sdk.Data.Base
{
    /// <summary>
    /// Configuration settings set by the source core.
    /// </summary>
    public interface ICoreConfigBase
    {
        /// <summary>
        /// Abstract UI elements that will be presented to the user for Settings, About, Legal notices, Donation links, etc.
        /// </summary>
        public IReadOnlyList<AbstractUIElementGroup> AbstractUIElements { get; }

        /// <summary>
        /// A local path or url pointing to a SVG file containing the logo for this core.
        /// </summary>
        public Uri LogoSvgUrl { get; }

        /// <summary>
        /// The player type supported by this core. <see cref="MediaPlayerType"/> for information on the different types.
        /// </summary>
        public MediaPlayerType PlaybackType { get; }
    }
}
