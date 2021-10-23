using System;
using System.IO;
using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Sdk.MediaPlayback
{
    /// <summary>
    /// Defines configuration for a single playback source.
    /// </summary>
    public interface IMediaSourceConfig
    {
        /// <summary>
        /// The <see cref="ICoreTrack"/> being played.
        /// </summary>
        public ICoreTrack Track { get; }

        /// <summary>
        /// An identifier for this source.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// A Uri representing a media source.
        /// </summary>
        public Uri? MediaSourceUri { get; }

        /// <summary>
        /// Holds the index of the <see cref="IMediaSourceConfig"/> in the list.
        /// </summary>
        public int? CurrentIndex { get; set; }

        /// <summary>
        /// A stream to a media file to be played.
        /// </summary>
        public Stream? FileStreamSource { get; }

        /// <summary>
        /// The content type of the <see cref="FileStreamSource"/>.
        /// </summary>
        public string? FileStreamContentType { get; }

        /// <summary>
        /// The <see cref="DateTime"/> that this source config expires, if applicable.
        /// </summary>
        public DateTime? ExpirationDate { get; }
    }
}
