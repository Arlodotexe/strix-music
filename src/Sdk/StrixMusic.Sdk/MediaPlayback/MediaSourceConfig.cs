using System;
using System.IO;
using StrixMusic.Sdk.Models.Core;

namespace StrixMusic.Sdk.MediaPlayback
{
    /// <inheritdoc cref="IMediaSourceConfig"/>
    public class MediaSourceConfig : IMediaSourceConfig
    {
        /// <summary>
        /// Constructs a new <see cref="MediaSourceConfig"/>
        /// </summary>
        /// <param name="track">The track that this media source is playing</param>
        /// <param name="id"><inheritdoc cref="Id"/></param>
        /// <param name="mediaSourceUri"><inheritdoc cref="MediaSourceUri"/></param>
        /// <param name="expirationDate"><inheritdoc cref="ExpirationDate"/></param>
        public MediaSourceConfig(ICoreTrack track, string id, Uri mediaSourceUri, DateTime? expirationDate = null)
        {
            Track = track;
            Id = id;
            MediaSourceUri = mediaSourceUri;
            ExpirationDate = expirationDate;
        }

        /// <summary>
        /// Constructs a new <see cref="MediaSourceConfig"/>
        /// </summary>
        /// <param name="track">The track that this media source is playing</param>
        /// <param name="id"><inheritdoc cref="Id"/></param>
        /// <param name="fileStream">A <see cref="Stream"/> to an audio file to play.</param>
        /// <param name="contentType">The content type for the <paramref name="fileStream"/>.</param>
        public MediaSourceConfig(ICoreTrack track, string id, Stream fileStream, string contentType)
        {
            Track = track;
            Id = id;
            FileStreamSource = fileStream;
            FileStreamContentType = contentType;
        }

        /// <inheritdoc />
        public ICoreTrack Track { get; }

        /// <inheritdoc/>
        public string Id { get; }

        /// <inheritdoc/>
        public Uri? MediaSourceUri { get; }

        /// <inheritdoc/>
        public DateTime? ExpirationDate { get; }

        /// <inheritdoc/>
        public Stream? FileStreamSource { get; }

        /// <inheritdoc/>
        public string? FileStreamContentType { get; }
    }
}
