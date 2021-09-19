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
    public interface ICoreConfigBase: IAsyncDisposable
    {
        /// <summary>
        /// Abstract UI elements that will be presented to the user for Settings, About, Legal notices, Donation links, etc.
        /// </summary>
        public IReadOnlyList<AbstractUICollection> AbstractUIElements { get; }

        /// <summary>
        /// The player type supported by this core. <see cref="MediaPlayerType"/> for information on the different types.
        /// </summary>
        public MediaPlayerType PlaybackType { get; }

        /// <summary>
        /// Raised when <see cref="AbstractUIElement"/> is changed.
        /// </summary>
        public event EventHandler? AbstractUIElementsChanged;
    }
}
