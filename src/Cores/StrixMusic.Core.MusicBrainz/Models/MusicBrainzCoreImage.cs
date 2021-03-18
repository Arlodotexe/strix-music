using System;
using System.Threading.Tasks;
using Hqub.MusicBrainz.API.Entities;
using StrixMusic.Core.MusicBrainz.Models.Enums;
using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Core.MusicBrainz.Models
{
    /// <summary>
    /// Wraps a <see cref="CoverArtArchive"/> as an <see cref="ICoreImage"/>.
    /// </summary>
    public class MusicBrainzCoreImage : ICoreImage
    {
        /// <summary>
        /// Creates a new instance of <see cref="MusicBrainzCoreImage"/>.
        /// </summary>
        /// <param name="sourceCore"><inheritdoc cref="ICoreMember.SourceCore"/></param>
        /// <param name="releaseId">The MusicBrainz Id for this release.</param>
        /// <param name="size">The size of the image to use.</param>
        public MusicBrainzCoreImage(ICore sourceCore, string releaseId, MusicBrainzImageSize size)
        {
            SourceCore = sourceCore;
            string url = $"https://coverartarchive.org/release/{releaseId}/front-{size}.jpg";

            Uri = new Uri(url, UriKind.Absolute);
            Height = Convert.ToDouble(size);
            Width = Convert.ToDouble(size);
        }

        /// <inheritdoc />
        public ICore SourceCore { get; }

        /// <inheritdoc />
        public Uri Uri { get; }

        /// <inheritdoc />
        public double Height { get; }

        /// <inheritdoc />
        public double Width { get; }

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            return default;
        }
    }
}
