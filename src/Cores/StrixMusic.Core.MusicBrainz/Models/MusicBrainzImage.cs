using System;
using Hqub.MusicBrainz.API.Entities;
using StrixMusic.Core.MusicBrainz.Models.Enums;
using StrixMusic.Sdk.Interfaces;

namespace StrixMusic.Core.MusicBrainz.Models
{
    /// <summary>
    /// Wraps a <see cref="CoverArtArchive"/> as an <see cref="IImage"/>.
    /// </summary>
    public class MusicBrainzImage : IImage
    {
        /// <summary>
        /// Creates a new instance of <see cref="MusicBrainzImage"/>.
        /// </summary>
        /// <param name="sourceCore"><inheritdoc cref="ICoreMember.SourceCore"/></param>
        /// <param name="releaseId">The MusicBrainz Id for this release.</param>
        /// <param name="size">The size of the image to use.</param>
        public MusicBrainzImage(ICore sourceCore, string releaseId, MusicBrainzImageSize size)
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
    }
}
