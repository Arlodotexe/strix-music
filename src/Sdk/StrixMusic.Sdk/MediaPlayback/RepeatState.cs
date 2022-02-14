// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

namespace StrixMusic.Sdk.MediaPlayback
{
    /// <summary>
    /// How songs will repeat in their context.
    /// </summary>
    public enum RepeatState
    {
        /// <summary>
        /// Neither track nor context will repeat.
        /// </summary>
        None,

        /// <summary>
        /// The current track will repeat when done playing.
        /// </summary>
        One,

        /// <summary>
        /// The current playback context (such as a Playlist) will start over when it finishes playing the last item.
        /// </summary>
        All,
    }
}
