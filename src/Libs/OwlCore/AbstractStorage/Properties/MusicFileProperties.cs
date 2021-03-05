using System;
using System.Collections.Generic;

namespace OwlCore.AbstractStorage
{
    /// <summary>
    /// Contains the music-related properties for a file or folder.
    /// </summary>
    /// <remarks>Adapted from <see href="https://docs.microsoft.com/en-us/uwp/api/Windows.Storage.FileProperties.MusicProperties?view=winrt-19041"/></remarks>
    public class MusicFileProperties
    {
        /// <summary>
        /// Gets or sets the name of the album that contains the song.
        /// </summary>
        public string? Album { get; set; }

        /// <summary>
        /// Gets or sets the name of the album artist of the song.
        /// </summary>
        public string? AlbumArtist { get; set; }

        /// <summary>
        /// Gets the artists that contributed to the song.
        /// </summary>
        public string? Artist { get; set; }

        /// <summary>
        /// Gets the bit rate of the song file.
        /// </summary>
        public uint? Bitrate { get; set; }

        /// <summary>
        /// Gets the composers of the song.
        /// </summary>
        public IReadOnlyList<string>? Composers { get; set; }

        /// <summary>
        /// Gets the conductors of the song.
        /// </summary>
        public IReadOnlyList<string>? Conductors { get; set; }

        /// <summary>
        /// Gets the duration of the song in milliseconds.
        /// </summary>
        public TimeSpan? Duration { get; set; }

        /// <summary>
        /// Gets the names of music genres that the song belongs to.
        /// </summary>
        public IReadOnlyList<string>? Genres { get; set; }

        /// <summary>
        /// Gets the producers of the song.
        /// </summary>
        public IReadOnlyList<string>? Producers { get; set; }

        /// <summary>
        /// Gets or sets the publisher of the song.
        /// </summary>
        public string? Publisher { get; set; }

        /// <summary>
        /// Gets or sets the rating associated with a music file.
        /// </summary>
        public uint? Rating { get; set; }

        /// <summary>
        /// Gets or sets the subtitle of the song.
        /// </summary>
        public string? Subtitle { get; set; }

        /// <summary>
        /// Gets or sets the title of the song.
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Gets or sets the track number of the song on the song's album.
        /// </summary>
        public uint? TrackNumber { get; set; }

        /// <summary>
        /// Gets the songwriters.
        /// </summary>
        public IReadOnlyList<string>? Writers { get; set; }

        /// <summary>
        /// Gets or sets the year that the song was released.
        /// </summary>
        public uint? Year { get; set; }
    }
}
