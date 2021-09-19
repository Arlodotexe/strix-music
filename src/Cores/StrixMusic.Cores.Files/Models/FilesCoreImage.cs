using System;
using System.Threading.Tasks;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Services.FileMetadataManager.Models;

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
        public LocalFilesCoreImage(ICore sourceCore, ImageMetadata imageMetadata)
        {
            _imageMetadata = imageMetadata;

            SourceCore = sourceCore;
        }

        /// <inheritdoc />
        public ICore SourceCore { get; }

        // TODO: Fix these placeholder exception (what should these properties do if _imageMetadata properties are null?).

        /// <inheritdoc />
        public Uri Uri => _imageMetadata.Uri ?? throw new InvalidOperationException();

        /// <inheritdoc />
        public double Height => _imageMetadata.Height ?? throw new InvalidOperationException();

        /// <inheritdoc />
        public double Width => _imageMetadata.Width ?? throw new InvalidOperationException();

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            return default;
        }
    }
}
