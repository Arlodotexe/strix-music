// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

namespace StrixMusic.Sdk.MediaPlayback
{
    /// <summary>
    /// The media player types supported by a core.
    /// </summary>
    public enum MediaPlayerType
    {
        /// <summary>
        /// No media playback supported.
        /// </summary>
        None,

        /// <summary>
        /// Indicates a standard, unencrypted playback source.
        /// </summary>
        Standard,

        /// <summary>
        /// Indicates remote playback only. All device manipulations will be delegated to the core.
        /// </summary>
        RemoteOnly,

        /// <summary>
        /// Indicates support for a PlayReady-enabled playback source.
        /// </summary>
        PlayReady,
    }
}
