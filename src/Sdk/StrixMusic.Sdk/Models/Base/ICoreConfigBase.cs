// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using OwlCore.AbstractUI.Models;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Sdk.Models.Base
{
    /// <summary>
    /// Configuration settings set by the source core.
    /// </summary>
    public interface ICoreConfigBase : IAsyncDisposable
    {
        /// <summary>
        /// Abstract UI elements that will be presented to the user for Settings, About, Legal notices, Donation links, etc.
        /// </summary>
        public AbstractUICollection AbstractUIElements { get; }

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
