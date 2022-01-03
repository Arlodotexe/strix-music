using System;
using System.Collections.Generic;

namespace StrixMusic.Sdk.Models.Base
{
    /// <summary>
    /// Contains lyrics for a track.
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