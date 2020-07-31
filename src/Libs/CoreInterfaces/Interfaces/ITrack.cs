using System;
using System.Collections.Generic;
using System.Text;
using CoreInterfaces.Enums;

namespace CoreInterfaces.Interfaces
{
    /// <summary>
    /// Base interface for track.
    /// </summary>
    public interface ITrack
    {
        /// <summary>
        /// Id for the track.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Images for the track.
        /// </summary>
        object Images { get; }

        /// <summary>
        /// An external link related to the track.
        /// </summary>
        Uri Url { get; }

        /// <summary>
        /// Identifies which type of track this is (song, podcast, etc).
        /// </summary>
        string Type { get; }

        /// <summary>
        /// Name of the track
        /// </summary>
        string Title { get; }

        /// <summary>
        /// An <see cref="IArtist"/> object that this track was created by.
        /// </summary>
        IArtist Artist { get; }

        /// <summary>
        /// An <see cref="IAlbum"/> object that this track belongs to
        /// </summary>
        IAlbum Album { get; }

        /// <summary>
        /// The date the track was released.
        /// </summary>
        DateTime DatePublished { get; }

        /// <summary>
        /// A list of <see cref="string"/> describing the genres for this track.
        /// </summary>
        List<string> Genere { get; }

        /// <summary>
        /// Position in a set, usually the album.
        /// </summary>
        int TrackNumber { get; }

        /// <summary>
        /// Number of the times this track has been played.
        /// </summary>
        int PlayCount { get; }

        /// <summary>
        /// The language this track is spoken in
        /// </summary>
        string Language { get; }

        /// <summary>
        /// The lyrics for this track.
        /// </summary>
        ILyrics Lyrics { get; }

        /// <summary>
        /// If this track contains explicit language.
        /// </summary>
        bool Explicit { get; }

        /// <summary>
        /// How long the track is.
        /// </summary>
        long Duration { get; }

        /// <summary>
        /// Provides comments about the track.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// If the song is currently playing this will represent the time in milliseconds that the song is currently playing.
        /// </summary>
        long Position { get; }

        /// <summary>
        /// If the song is currently playing this will represent the time in milliseconds that the song is currently playing.
        /// </summary>
        TrackState State { get; }

        /// <summary>
        /// Attempts to play the track.
        /// </summary>
        /// <returns><see langword="true"/> if the <see cref="ITrack"/> was played successfully, <see langword="false"/> otherwise.</returns>
        bool Play();

        /// <summary>
        /// Attempts to pause the track.
        /// </summary>
        /// <returns><see langword="true"/> if the <see cref="ITrack"/> was paused successfully, <see langword="false"/> otherwise.</returns>
        bool Pause();

        /// <summary>
        /// Seeks the track to a given timestamp.
        /// </summary>
        /// <param name="position">Time in milliseconds to seek the song to.</param>
        /// <returns><see langword="true"/> if the <see cref="ITrack"/> was seeked to successfully, <see langword="false"/> otherwise.</returns>
        bool Seek(long position);
    }
}
