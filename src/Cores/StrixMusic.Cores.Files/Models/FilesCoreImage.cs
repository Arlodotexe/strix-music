using System;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using StrixMusic.Sdk.FileMetadataManagement.Models;
using StrixMusic.Sdk.Models.Core;

namespace StrixMusic.Cores.Files.Models
{
    ///<inheritdoc/>
    public sealed class FilesCoreImage : ICoreImage
    {
        private readonly ImageMetadata _imageMetadata;

        /// <summary>
        /// Creates a new instance of <see cref="FilesCoreImage"/>.
        /// </summary>
        /// <param name="sourceCore">The source core.</param>
        /// <param name="imageMetadata">The image metadata to wrap around.</param>
        public FilesCoreImage(ICore sourceCore, ImageMetadata imageMetadata)
        {
            _imageMetadata = imageMetadata;

            // Guard.IsNotNull() wasn't working here. Fallback to ThrowHelper.
            Uri = imageMetadata.Uri ?? ThrowHelper.ThrowArgumentNullException<Uri>(nameof(imageMetadata.Uri));
            Height = (double?)imageMetadata.Height ?? ThrowHelper.ThrowArgumentNullException<int>(nameof(imageMetadata.Height));
            Width = (double?)imageMetadata.Width ?? ThrowHelper.ThrowArgumentNullException<int>(nameof(imageMetadata.Width));

            SourceCore = sourceCore;
        }

        /// <inheritdoc />
        public ICore SourceCore { get; }

        // TODO: Fix these placeholder exception (what should these properties do if _imageMetadata properties are null?).

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
