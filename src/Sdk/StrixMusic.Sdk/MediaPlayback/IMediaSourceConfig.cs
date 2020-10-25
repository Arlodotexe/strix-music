using System;
using System.IO;
using StrixMusic.Sdk.Core.Data;

namespace StrixMusic.Sdk.MediaPlayback
{
    /// <summary>
    /// Defines configuration for a single playback source.
    /// </summary>
    public interface IMediaSourceConfig
    {
        /// <summary>
        /// The <see cref="ITrack"/> being played.
        /// </summary>
        public ITrack Track { get; }

        /// <summary>
        /// An identifier for this source.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// A Uri representing a media source.
        /// </summary>
        public Uri? MediaSourceUri { get; }

        /// <summary>
        /// A stream to a media file to be played.
        /// </summary>
        public Stream? FileStreamSource { get; }

        /// <summary>
        /// The <see cref="DateTime"/> that this source config expires, if applicable.
        /// </summary>
        public DateTime? ExpirationDate { get; }
    }
}
