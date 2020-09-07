using System;
using System.Collections.Generic;
using StrixMusic.Sdk.Interfaces.Storage;

namespace StrixMusic.Sdk.Models.Storage
{
    /// <inheritdoc cref="IMusicFileProperties" />
    public class MusicFileProperties : IMusicFileProperties
    {
        /// <inheritdoc />
        public string? Album { get; set; }

        /// <inheritdoc />
        public string? AlbumArtist { get; set; }

        /// <inheritdoc />
        public string? Artist { get; set; }

        /// <inheritdoc />
        public uint? Bitrate { get; set; }

        /// <inheritdoc />
        public IReadOnlyList<string>? Composers { get; set; }

        /// <inheritdoc />
        public IReadOnlyList<string>? Conductors { get; set; }

        /// <inheritdoc />
        public TimeSpan? Duration { get; set; }

        /// <inheritdoc />
        public IReadOnlyList<string>? Genre { get; set; }

        /// <inheritdoc />
        public IReadOnlyList<string>? Producers { get; set; }

        /// <inheritdoc />
        public string? Publisher { get; set; }

        /// <inheritdoc />
        public uint? Rating { get; set; }

        /// <inheritdoc />
        public string? Subtitle { get; set; }

        /// <inheritdoc />
        public string? Title { get; set; }

        /// <inheritdoc />
        public uint? TrackNumber { get; set; }

        /// <inheritdoc />
        public IReadOnlyList<string>? Writers { get; set; }

        /// <inheritdoc />
        public uint? Year { get; set; }
    }
}
