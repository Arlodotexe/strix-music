using System;
using System.Collections.Generic;

namespace StrixMusic.CoreInterfaces.Interfaces
{
    /// <summary>
    /// Metadata about a track.
    /// </summary>
    public interface ITrack : IPlayable
    {
        /// <summary>
        /// An external link related to the track.
        /// </summary>
        Uri? Url { get; }

        /// <summary>
        /// Identifies which type of track this is (song, podcast, etc).
        /// </summary>
        string Type { get; }

        /// <summary>
        /// A list of <see cref="IArtist"/>s that this track was created by.
        /// </summary>
        IReadOnlyList<IArtist> Artists { get; }

        /// <summary>
        /// An <see cref="IAlbum"/> object that this track belongs to.
        /// </summary>
        IAlbum? Album { get; }

        /// <summary>
        /// The date the track was released.
        /// </summary>
        DateTime? DatePublished { get; }

        /// <summary>
        /// A list of <see cref="string"/> describing the genres for this track.
        /// </summary>
        IReadOnlyList<string>? Genres { get; }

        /// <summary>
        /// Position in a set, usually the album.
        /// </summary>
        int? TrackNumber { get; }

        /// <summary>
        /// Number of the times this track has been played.
        /// </summary>
        int? PlayCount { get; }

        /// <summary>
        /// The language this track is spoken in.
        /// </summary>
        string? Language { get; }

        /// <summary>
        /// The lyrics for this track.
        /// </summary>
        ILyrics? Lyrics { get; }

        /// <summary>
        /// If this track contains explicit language.
        /// </summary>
        bool IsExplicit { get; }

        /// <summary>
        /// How long the track is in.
        /// </summary>
        TimeSpan Duration { get; }

    }
}
