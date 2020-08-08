using System;
using System.IO;
using StrixMusic.CoreInterfaces.Interfaces.MediaPlayback;

namespace StrixMusic.CoreInterfaces.Models.MediaPlayback
{
    /// <summary>
    /// Defines configuration for a single PlayReady-enabled playback source.
    /// </summary>
    public class PlayReadyMediaSourceConfig : IMediaSourceConfig
    {
        /// <summary>
        /// Constructs a new <see cref="PlayReadyMediaSourceConfig"/>
        /// </summary>
        /// <param name="id"><inheritdoc cref="Id"/></param>
        /// <param name="licenseAcquisitionUri"><inheritdoc cref="LicenseAcquisitionUri"/></param>
        /// <param name="mediaSourceUri"><inheritdoc cref="MediaSourceUri"/></param>
        /// <param name="expirationDate"><inheritdoc cref="ExpirationDate"/></param>
        public PlayReadyMediaSourceConfig(string id, Uri licenseAcquisitionUri, Uri mediaSourceUri, DateTime expirationDate)
        {
            Id = id;
            MediaSourceUri = mediaSourceUri;
            ExpirationDate = expirationDate;
            LicenseAcquisitionUri = licenseAcquisitionUri;
        }

        /// <summary>
        /// Constructs a new <see cref="MediaSourceConfig"/>
        /// </summary>
        /// <param name="id"><inheritdoc cref="Id"/></param>
        /// <param name="licenseAcquisitionUri"><inheritdoc cref="LicenseAcquisitionUri"/></param>
        public PlayReadyMediaSourceConfig(string id, Stream fileStream, Uri licenseAcquisitionUri)
        {
            Id = id;
            FileStreamSource = fileStream;
            LicenseAcquisitionUri = licenseAcquisitionUri;
        }

        /// <inheritdoc/>
        public string Id { get; }

        /// <inheritdoc/>
        public Uri? MediaSourceUri { get; }

        /// <inheritdoc/>
        public DateTime? ExpirationDate { get; }

        /// <inheritdoc/>
        public Stream? FileStreamSource { get; }

        /// <summary>
        /// The URI used to acquire the PlayReady license.
        /// </summary>
        public Uri LicenseAcquisitionUri { get; }
    }
}
