using System;
using System.Threading.Tasks;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Services.FileMetadataManager.Models;

namespace StrixMusic.Core.LocalFiles.Models
{
    ///<inheritdoc/>
    public class LocalFilesCoreImage : ICoreImage
    {
        private readonly ImageMetadata _imageMetadata;

        /// <summary>
        /// Creates a new instance of <see cref="LocalFilesCoreImage"/>.
        /// </summary>
        /// <param name="sourceCore">The source core.</param>
        /// <param name="imageMetadata">The image metadata to wrap around.</param>
        public LocalFilesCoreImage(ICore sourceCore, ImageMetadata imageMetadata)
        {
            _imageMetadata = imageMetadata;

            SourceCore = sourceCore;

            if (imageMetadata.Size != null)
            {
                var size = (double) imageMetadata.Size;
                Width = size;
                Height = size;
            }
            else
            {
                // TODO: Fix this placeholder exception (what should this constructor do if _imageMetadata.Size is null?).
                throw new InvalidOperationException();
            }
        }

        /// <inheritdoc />
        public ICore SourceCore { get; }

        /// <inheritdoc />
        // TODO: Fix this placeholder exception (what should this property do if _imageMetadata.Uri is null?).
        public Uri Uri => _imageMetadata.Uri ?? throw new InvalidOperationException();

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
