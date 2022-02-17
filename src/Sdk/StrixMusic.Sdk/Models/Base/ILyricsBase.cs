// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Collections.Generic;

namespace StrixMusic.Sdk.Models.Base
{
    /// <summary>
    /// Contains the lyrics to a track.
    /// </summary>
    public interface ILyricsBase
    {
        /// <summary>
        /// Timestamped lyrics. The Key is a point in the song, and the value is the lyric at that position.
        /// </summary>
        Dictionary<TimeSpan, string>? TimedLyrics { get; }

        /// <summary>
        /// A simple text wall containing all the lyrics for this song.
        /// </summary>
        string? TextLyrics { get; }
    }
}