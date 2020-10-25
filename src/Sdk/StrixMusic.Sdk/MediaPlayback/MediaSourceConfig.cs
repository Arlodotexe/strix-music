using System;
using System.IO;
using StrixMusic.Sdk.Core.Data;

namespace StrixMusic.Sdk.MediaPlayback
{
    /// <inheritdoc/>
    public class MediaSourceConfig : IMediaSourceConfig
    {
        /// <summary>
        /// Constructs a new <see cref="MediaSourceConfig"/>
        /// </summary>
        /// <param name="track">The track that this media source is playing</param>
        /// <param name="id"><inheritdoc cref="Id"/></param>
        /// <param name="mediaSourceUri"><inheritdoc cref="MediaSourceUri"/></param>
        /// <param name="expirationDate"><inheritdoc cref="ExpirationDate"/></param>
        public MediaSourceConfig(ITrack track, string id, Uri mediaSourceUri, DateTime expirationDate)
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
        /// <param name="fileStream"></param>
        public MediaSourceConfig(ITrack track, string id, Stream fileStream)
        {
            Track = track;
            Id = id;
            FileStreamSource = fileStream;
        }

        /// <inheritdoc />
        public ITrack Track { get; }

        /// <inheritdoc/>
        public string Id { get; }

        /// <inheritdoc/>
        public Uri? MediaSourceUri { get; }

        /// <inheritdoc/>
        public DateTime? ExpirationDate { get; }

        /// <inheritdoc/>
        public Stream? FileStreamSource { get; }
    }
}
