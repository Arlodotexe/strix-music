using System;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.FileMetadata.Models;

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
            Height = (double?)imageMetadata.Height ?? ThrowHelper.ThrowArgumentNullException<int>(nameof(imageMetadata.Height));
            Width = (double?)imageMetadata.Width ?? ThrowHelper.ThrowArgumentNullException<int>(nameof(imageMetadata.Width));

            SourceCore = sourceCore;
        }

        /// <inheritdoc />
        public ICore SourceCore { get; }

        /// <inheritdoc />
        public async Task<Stream> OpenStreamAsync()
        {
            var fileMetadataManager = ((FilesCore)SourceCore).FileMetadataManager;
            Guard.IsNotNull(fileMetadataManager);
            Guard.IsNotNull(_imageMetadata.Id);

            var stream = await fileMetadataManager.GetImageStreamById(_imageMetadata.Id);
            Guard.IsNotNull(stream);

            return stream;
        }

        /// <inheritdoc />
        public string? MimeType => _imageMetadata.MimeType;

        /// <inheritdoc />
        public double? Height { get; }

        /// <inheritdoc />
        public double? Width { get; }

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            return default;
        }
    }
}
