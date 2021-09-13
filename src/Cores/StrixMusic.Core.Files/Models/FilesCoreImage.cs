using System;
using System.Threading.Tasks;
using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Cores.Files.Models
{
    ///<inheritdoc/>
    public class FilesCoreImage : ICoreImage
    {
        /// <summary>
        /// Creates a new instance of <see cref="FilesCoreImage"/>.
        /// </summary>
        /// <param name="sourceCore"><inheritdoc cref="ICoreMember.SourceCore"/></param>
        /// <param name="uri">A <see cref="System.Uri"/> pointing to the image file on the disk.</param>
        /// <param name="width">The width of the image, or <see langword="null"/> if not available.</param>
        /// <param name="height">The height of the image, or <see langword="null"/> if not available.</param>
        public FilesCoreImage(ICore sourceCore, Uri uri, double? width = null, double? height = null)
        {
            SourceCore = sourceCore;
            Uri = uri;
            Width = width ?? 250;
            Height = height ?? 250;
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
