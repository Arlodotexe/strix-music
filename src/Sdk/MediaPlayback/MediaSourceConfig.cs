using System;
using System.IO;

namespace StrixMusic.Sdk.MediaPlayback
{
    /// <inheritdoc/>
    public class MediaSourceConfig : IMediaSourceConfig
    {
        /// <summary>
        /// Constructs a new <see cref="MediaSourceConfig"/>
        /// </summary>
        /// <param name="id"><inheritdoc cref="Id"/></param>
        /// <param name="mediaSourceUri"><inheritdoc cref="MediaSourceUri"/></param>
        /// <param name="expirationDate"><inheritdoc cref="ExpirationDate"/></param>
        public MediaSourceConfig(string id, Uri mediaSourceUri, DateTime expirationDate)
        {
            Id = id;
            MediaSourceUri = mediaSourceUri;
            ExpirationDate = expirationDate;
        }

        /// <summary>
        /// Constructs a new <see cref="MediaSourceConfig"/>
        /// </summary>
        /// <param name="id"><inheritdoc cref="Id"/></param>
        /// <param name="fileStream"></param>
        public MediaSourceConfig(string id, Stream fileStream)
        {
            Id = id;
            FileStreamSource = fileStream;
        }

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
