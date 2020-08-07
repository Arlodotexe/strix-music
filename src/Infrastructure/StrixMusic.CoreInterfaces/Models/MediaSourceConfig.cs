using System;
using System.IO;
using StrixMusic.CoreInterfaces.Interfaces.MediaPlayback;

namespace StrixMusic.CoreInterfaces.Interfaces.Models
{
    /// <inheritdoc/>
    public class MediaSourceConfig : IMediaSourceConfig
    {
        /// <summary>
        /// Constructs a new <see cref="MediaSourceConfig"/>
        /// </summary>
        /// <param name="id"><inheritdoc cref="Id"/></param>
        /// <param name="licenseAcquisitionUri"><inheritdoc cref="LicenseAcquisitionUri"/></param>
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
        /// <param name="licenseAcquisitionUri"><inheritdoc cref="LicenseAcquisitionUri"/></param>
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
