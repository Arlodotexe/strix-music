using System;
using System.Collections.Generic;
using System.Text;
using StrixMusic.Sdk.Interfaces;

namespace StrixMusic.Sdk.Enums
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
        /// The current <see cref="ITrack"/> will repeat when done playing.
        /// </summary>
        One,

        /// <summary>
        /// The current context (<see cref="IAlbum"/>, <see cref="IPlaylist"/>, etc) will start over when it finishes playing the last item.
        /// </summary>
        All,
    }
}
